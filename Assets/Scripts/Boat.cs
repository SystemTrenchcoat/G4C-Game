using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : BoidAgent_P4
{
    [Header("References")]
    public GameObject soundWavePrefab;
    [SerializeField] private SoundWaveCooldownUI cooldownUI;
    private float nextSoundWaveTime = 0f;

    [Header("Movement Settings")]
    public float followStrength = 8f;
    public float movementDamping = 0.05f;          // boat drifts slightly
    [SerializeField] private float fullSpeedDistance = 3f; // distance where speed is max
    [SerializeField] private float distanceFactorMultiplier = 2f; // controls U-curve steepness
    [SerializeField] private float stopThreshold = 0.1f; // deadzone near mouse

    [Header("Fish Catch Settings")]
    public int maxCaughtFish = 5;
    public float slowPerFish = 0.2f;               // slowdown per fish
    public int basePressNeeded = 6;
    [SerializeField] private int additionalPressPerFish = 2;
    [SerializeField] private float fishOffsetRange = 0.5f;

    public KeyCode key1 = KeyCode.A;
    public KeyCode key2 = KeyCode.D;

    private bool expectingKey1 = true;
    private int pressCounter = 0;
    private List<Fish> caughtFish = new List<Fish>();

    // ---------------- SPEED MULTIPLIER ----------------
    private float GetSpeedMultiplier()
    {
        float mult = 1f - (slowPerFish * caughtFish.Count);
        return Mathf.Max(mult, 0.1f);
    }

    public float GetSlowdownPercent()
    {
        return GetSpeedMultiplier() * 100f;
    }

    // ---------------- BOAT MOVEMENT ----------------
    protected override Vector2 CalculatedSteering()
    {
        float speedMult = GetSpeedMultiplier();
        float adjustedMaxSpeed = maxSpeed * speedMult;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 toMouse = mousePos - (Vector2)transform.position;
        float distance = toMouse.magnitude;

        // deadzone near mouse
        if (distance < stopThreshold)
        {
            velocity *= 0.95f; // drift slightly
            return Vector2.zero;
        }

        // distance-based easing with steepness control (U-curve)
        float distanceRatio = distance / fullSpeedDistance;
        float distanceFactor = Mathf.Clamp01(Mathf.Pow(distanceRatio, distanceFactorMultiplier));
        float currentSpeed = adjustedMaxSpeed * distanceFactor;

        Vector2 desiredVelocity = toMouse.normalized * currentSpeed;
        Vector2 steering = desiredVelocity - velocity;
        steering *= followStrength;

        // damping for natural boat feel
        velocity *= (1f - movementDamping * Time.deltaTime);

        return Vector2.ClampMagnitude(steering, maxForce);
    }


    // ---------------- UNITY METHODS ----------------
    private void Start()
    {
        if (cooldownUI != null)
            cooldownUI.SetCooldownDuration(1f);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextSoundWaveTime)
        {
            Instantiate(soundWavePrefab, transform.position, Quaternion.identity);
            nextSoundWaveTime = Time.time + 1f;

            if (cooldownUI != null)
                cooldownUI.StartCooldown();
        }

        if (caughtFish.Count > 0)
            HandleMash();
    }

    // ---------------- MASH LOGIC ----------------
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

        int required = basePressNeeded + (caughtFish.Count * additionalPressPerFish);

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

        fish.enabled = true;
        fish.isLeader = false;
        fish.leaderFish = null;

        GameManager.instance.OnFishReleased();

        // tutorial only notification
        if (TutorialManager.instance != null && TutorialManager.instance.IsTutorialFish(fish))
        {
            TutorialManager.instance.OnFishReleased_Tutorial();
        }
    }

    // ---------------- CATCHING FISH ----------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fish fish = collision.GetComponent<Fish>();
        if (fish != null)
        {
            CatchFish(fish);
            return;
        }
    }

    private void CatchFish(Fish fish)
    {
        if (caughtFish.Contains(fish)) return;
        if (caughtFish.Count >= maxCaughtFish) return;

        fish.enabled = false;
        fish.transform.SetParent(transform);
        fish.transform.localPosition = new Vector3(
            Random.Range(-fishOffsetRange, fishOffsetRange),
            Random.Range(-fishOffsetRange, fishOffsetRange),
            0f
        );

        caughtFish.Add(fish);
        GameManager.instance.OnFishCaught();
    }

    // ---------------- UI GETTERS ----------------
    public int GetCaughtFishCount() => caughtFish.Count;
    public int GetMashProgress() => pressCounter;
}




