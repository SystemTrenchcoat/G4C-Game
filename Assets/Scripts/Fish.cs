using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Collections;
using UnityEngine;

public class Fish : BoidAgent_P4
{
    private Vector2 wanderTarget;
    private Trash targetTrash;
    private float touchTimer = 0f;
    private float deathTime;
    private bool isScared = false;
    private float scaredTimer = 0f;
    private float scaredDuration = 4f; // time fish refuse to approach trash

    // Instead of one SpriteRenderer, we use an array
    private SpriteRenderer[] renderers;
    private Color normalColor = Color.green;

    private void Start()
    {
        wanderTarget = Random.insideUnitCircle.normalized * neighborRadius;
        deathTime = Random.Range(5f, 8f);

        // get all SpriteRenderers in this fish (for both animation sprites)
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    protected override Vector2 CalculatedSteering()
    {
        // If scared, avoid all trash
        if (isScared)
        {
            scaredTimer += Time.deltaTime;
            if (scaredTimer >= scaredDuration)
            {
                isScared = false;
                SetFishColor(normalColor); // stop flashing
            }

            // flee from nearest trash while scared
            Trash nearestTrash = FindNearestTrash(6f);
            if (nearestTrash != null)
            {
                return Flee(nearestTrash.transform.position);
            }

            return Wander();
        }

        // Normal behavior
        Trash nearestTrashNormal = FindNearestTrash(5f);
        Vector2 steering = Vector2.zero;

        if (nearestTrashNormal != null && Random.value < 0.5f)
        {
            steering += Seek(nearestTrashNormal.transform.position);
        }
        else
        {
            steering += Wander();
        }

        return Vector2.ClampMagnitude(steering, maxForce);
    }

    private Vector2 Wander()
    {
        wanderTarget += Random.insideUnitCircle * 0.5f;
        wanderTarget = wanderTarget.normalized * neighborRadius;
        Vector2 targetPosition = (Vector2)transform.position + wanderTarget;
        return Seek(targetPosition);
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

    private Vector2 Flee(Vector2 threatPosition)
    {
        Vector2 desired = ((Vector2)transform.position - threatPosition).normalized * maxSpeed;
        return desired - velocity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isScared) return; // ignore while scared

        if (collision.GetComponent<Trash>() != null)
        {
            touchTimer += Time.deltaTime;

            // Flash red on all sprite renderers
            SetFishColor(Color.Lerp(normalColor, Color.red, Mathf.PingPong(Time.time * 10f, 1f)));

            if (touchTimer >= deathTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Trash>() != null)
        {
            touchTimer = 0f;
            SetFishColor(normalColor);
        }
    }

    // called when hit by sound wave
    public void OnSoundWaveHit(Vector2 center)
    {
        Debug.Log("Sound wave hit! Scared!");
        isScared = true;
        scaredTimer = 0f;
        StartCoroutine(FlashYellow());

        // add a quick "burst" away from the wave center
        Vector2 fleeForce = Flee(center) * 1.5f;
        ApplyForce(fleeForce);
    }

    private IEnumerator FlashYellow()
    {
        float timer = 0f;
        while (timer < scaredDuration)
        {
            SetFishColor(Color.Lerp(normalColor, Color.yellow, Mathf.PingPong(Time.time * 10f, 1f)));
            timer += Time.deltaTime;
            yield return null;
        }
        SetFishColor(normalColor);
    }

    // helper to set color on all SpriteRenderers
    private void SetFishColor(Color color)
    {
        if (renderers == null) return;
        foreach (SpriteRenderer r in renderers)
        {
            if (r != null) 
            {
                r.color = color;
            }
        }
    }
}

