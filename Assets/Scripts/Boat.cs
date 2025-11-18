using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boat : BoidAgent_P4
{
    public GameObject soundWavePrefab;
    [SerializeField] private float followStrength = 8f;
    [SerializeField] private float damping = 5f;
    [SerializeField] private float soundWaveCooldown = 1f;
    [SerializeField] private SoundWaveCooldownUI cooldownUI;
    private float nextSoundWaveTime = 0f;

    // -----------------------
    // Fish Catch + Mash
    // -----------------------
    [Header("Fish Catch Settings")]
    public int maxCaughtFish = 5;
    public float slowPerFish = 0.18f;
    public int basePressNeeded = 6;

    public KeyCode key1 = KeyCode.A;
    public KeyCode key2 = KeyCode.D;

    private bool expectingKey1 = true;
    private int pressCounter = 0;
    private List<Fish> caughtFish = new List<Fish>();

    // -----------------------
    // Movement
    // -----------------------
    protected override Vector2 CalculatedSteering()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 desiredVelocity = (mousePos - (Vector2)transform.position).normalized * maxSpeed;
        Vector2 steering = desiredVelocity - velocity;
        steering *= followStrength;

        velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * damping);

        return Vector2.ClampMagnitude(steering, maxForce);
    }

    private void Start()
    {
        if (cooldownUI != null)
            cooldownUI.SetCooldownDuration(soundWaveCooldown);
    }

    private void LateUpdate()
    {
        // Soundwave right-click
        if (Input.GetMouseButtonDown(0) && Time.time >= nextSoundWaveTime)
        {
            Instantiate(soundWavePrefab, transform.position, Quaternion.identity);
            nextSoundWaveTime = Time.time + soundWaveCooldown;

            if (cooldownUI != null)
                cooldownUI.StartCooldown();
        }

        // Mash only active if fish caught
        if (caughtFish.Count > 0)
            HandleMash();
    }

    // -----------------------
    // Mash logic
    // -----------------------
    private void HandleMash()
    {
        bool pressed = false;

        if (expectingKey1)
        {
            if (Input.GetKeyDown(key1))
            {
                pressCounter++;
                expectingKey1 = false;
                pressed = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(key2))
            {
                pressCounter++;
                expectingKey1 = true;
                pressed = true;
            }
        }

        if (!pressed) return;

        int required = basePressNeeded + (caughtFish.Count * 2);

        if (pressCounter >= required)
        {
            ReleaseFish();
            pressCounter = 0;
            expectingKey1 = true;
        }
    }

    private void ReleaseFish()
    {
        if (caughtFish.Count == 0) return;

        Fish fish = caughtFish[0];
        caughtFish.RemoveAt(0);

        fish.transform.SetParent(null);
        fish.transform.position = transform.position;

        // restore fish behavior
        fish.enabled = true;
        fish.isLeader = false;
        fish.leaderFish = null;
    }

    // -----------------------
    // Catching fish
    // -----------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fish fish = collision.GetComponent<Fish>();
        if (fish != null)
        {
            CatchFish(fish);
            return;
        }

        Trash trash = collision.GetComponent<Trash>();
        if (trash != null)
        {
            Destroy(trash.gameObject);
        }
    }

    private void CatchFish(Fish fish)
    {
        if (caughtFish.Contains(fish)) return;
        if (caughtFish.Count >= maxCaughtFish) return;

        fish.enabled = false;
        fish.transform.SetParent(transform);
        fish.transform.localPosition = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            0f
        );

        caughtFish.Add(fish);
    }

    // -----------------------
    // Public UI getters
    // -----------------------
    public int GetCaughtFishCount() => caughtFish.Count;
    public int GetMashProgress() => pressCounter;
}



