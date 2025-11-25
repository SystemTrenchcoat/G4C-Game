using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fish : BoidAgent_P4
{
    private Vector2 wanderTarget;
    private Trash targetTrash;
    private float touchTimer = 0f;
    private float deathTime;
    private bool isScared = false;
    private float scaredTimer = 0f;
    private float scaredDuration = 4f;
    private Vector2 soundWaveOrigin;

    private SpriteRenderer[] renderers;
    private Color[] defaultColors;
    private Coroutine flashCoroutine;

    public bool isLeader = false;
    public Fish leaderFish = null;
    public float followDistance = 2f;

    [SerializeField] private Vector2 worldMin = new Vector2(-20, -12);
    [SerializeField] private Vector2 worldMax = new Vector2(20, 12);
    [SerializeField] private float boundaryForce = 10f;
    [SerializeField] private float boundaryPadding = 2f;

    private void Start()
    {
        wanderTarget = Random.insideUnitCircle.normalized * neighborRadius;
        deathTime = Random.Range(5f, 8f);

        // get all SpriteRenderers and store default colors
        renderers = GetComponentsInChildren<SpriteRenderer>();
        defaultColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            defaultColors[i] = renderers[i].color;
    }

    protected override Vector2 CalculatedSteering()
    {
        // ------------------ Scared behavior ------------------
        if (isScared)
        {
            scaredTimer += Time.deltaTime;
            if (scaredTimer >= scaredDuration)
            {
                isScared = false;
                SetFishColorDefault();
            }

            // flee directly from soundwave origin
            return FleeFromSoundWave() * 1.5f + BoundaryAvoidance();
        }

        // ------------------ Follower behavior ------------------
        if (!isLeader && leaderFish != null)
        {
            float dist = Vector2.Distance(transform.position, leaderFish.transform.position);
            Vector2 offset = (Vector2)leaderFish.transform.position + Random.insideUnitCircle * 0.3f;
            Vector2 desiredForce = Seek(offset);
            desiredForce *= (dist > followDistance) ? 1.2f : 0.6f;

            Vector2 leaderDir = leaderFish.GetVelocity();
            if (leaderDir.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(leaderDir.y, leaderDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.AngleAxis(targetAngle, Vector3.forward),
                    Time.deltaTime * 5f
                );
            }

            return desiredForce + BoundaryAvoidance();
        }

        // ------------------ Leader / Normal behavior ------------------
        Trash nearestTrash = FindNearestTrash(5f);
        Vector2 steering = Vector2.zero;

        if (nearestTrash != null && Random.value < 0.5f)
            steering += Seek(nearestTrash.transform.position);
        else
            steering += Wander();

        steering += BoundaryAvoidance();
        return Vector2.ClampMagnitude(steering, maxForce);
    }

    private Vector2 Wander()
    {
        wanderTarget += Random.insideUnitCircle * 0.5f;
        wanderTarget = wanderTarget.normalized * neighborRadius;
        Vector2 targetPos = (Vector2)transform.position + wanderTarget;
        return Seek(targetPos);
    }

    private Trash FindNearestTrash(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        Trash nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            Trash t = hit.GetComponent<Trash>();
            if (t != null)
            {
                float dist = Vector2.Distance(transform.position, t.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = t;
                }
            }
        }
        return nearest;
    }

    private Vector2 FleeFromSoundWave()
    {
        Vector2 fleeDir = (Vector2)transform.position - soundWaveOrigin;
        if (fleeDir.sqrMagnitude < 0.01f)
            fleeDir = Random.insideUnitCircle.normalized;

        return fleeDir.normalized * maxSpeed - velocity;
    }

    private Vector2 BoundaryAvoidance()
    {
        Vector2 pos = transform.position;
        Vector2 force = Vector2.zero;

        if (pos.x < worldMin.x + boundaryPadding) force.x = boundaryForce;
        if (pos.x > worldMax.x - boundaryPadding) force.x = -boundaryForce;
        if (pos.y < worldMin.y + boundaryPadding) force.y = boundaryForce;
        if (pos.y > worldMax.y - boundaryPadding) force.y = -boundaryForce;

        return force;
    }

    // ------------------ Trash contact ------------------
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isScared) return;

        Trash trash = collision.GetComponent<Trash>();
        if (trash != null)
        {
            touchTimer += Time.deltaTime;
            SetFishColor(Color.Lerp(defaultColors[0], Color.red, Mathf.PingPong(Time.time * 10f, 1f)));

            if (touchTimer >= deathTime)
                Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Trash>() != null)
        {
            touchTimer = 0f;
            SetFishColorDefault();
        }
    }

    // ------------------ Soundwave hit ------------------
    public void OnSoundWaveHit(Vector2 center)
    {
        isScared = true;
        scaredTimer = 0f;
        soundWaveOrigin = center;

        // Stop red flash
        SetFishColorDefault();

        // Start cyan flashing
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashCyan());
    }

    private IEnumerator FlashCyan()
    {
        float timer = 0f;
        while (timer < scaredDuration)
        {
            float t = Mathf.PingPong(Time.time * 10f, 1f);
            Color flash = Color.Lerp(defaultColors[0], Color.cyan, t);
            SetFishColor(flash);
            timer += Time.deltaTime;
            yield return null;
        }
        SetFishColorDefault();
        isScared = false;
    }

    private void SetFishColor(Color c)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
            if (r != null) r.color = c;
    }

    private void SetFishColorDefault()
    {
        if (renderers == null || defaultColors == null) return;
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].color = defaultColors[i];
    }
}


