using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : BoidAgent_P4
{
    [Header("Collection Settings")]
    public float collectDuration = 3f;
    private float collectTimer = 0f;
    private bool isCollecting = false;
    private bool boatInside = false;

    private SpriteRenderer sr;
    private Color originalColor;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    protected override Vector2 CalculatedSteering()
    {
        return Wander();
    }

    private Vector2 Wander()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
        Vector2 targetPos = (Vector2)transform.position + randomOffset;
        return Seek(targetPos);
    }

    // ----------------------------------------
    // SECOND UPDATE LOOP (does not override Agent_P4.Update)
    // ----------------------------------------
    private void LateUpdate()
    {
        if (!isCollecting)
            return;

        if (!boatInside)
        {
            ResetCollection();
            return;
        }

        collectTimer += Time.deltaTime;

        // Flash yellow progress
        FlashDuringCollection();

        if (collectTimer >= collectDuration)
        {
            FinishCollection();
        }
    }

    private void FlashDuringCollection()
    {
        if (sr == null) return;

        float progress = collectTimer / collectDuration;

        // Flash frequency = 3 flashes per second
        float flashesPerSecond = 3f;

        float t = Mathf.Sin(Time.time * flashesPerSecond * Mathf.PI * 2f);
        t = Mathf.InverseLerp(-1f, 1f, t);

        Color target = Color.yellow;
        sr.color = Color.Lerp(originalColor, target, t);
    }

    private void FinishCollection()
    {
        GameManager.instance.OnTrashCollected();
        Destroy(gameObject);
    }

    private void ResetCollection()
    {
        isCollecting = false;
        collectTimer = 0f;

        if (sr != null)
            sr.color = originalColor;
    }

    // ----------------------------------------
    // TRIGGERS
    // ----------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Boat>() != null)
        {
            boatInside = true;
            isCollecting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Boat>() != null)
        {
            boatInside = false;
        }
    }
}

