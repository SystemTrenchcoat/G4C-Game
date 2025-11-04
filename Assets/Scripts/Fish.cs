using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fish : BoidAgent_P4
{
    private Vector2 wanderTarget;
    private Trash targetTrash;
    private float touchTimer = 0f;
    private float deathTime; // random between 5–8 seconds

    private void Start()
    {
        wanderTarget = Random.insideUnitCircle.normalized * neighborRadius;
        deathTime = Random.Range(5f, 8f);
    }

    protected override Vector2 CalculatedSteering()
    {
        // 50/50 chance to chase trash if near
        Trash nearestTrash = FindNearestTrash(5f);
        Vector2 steering = Vector2.zero;

        if (nearestTrash != null && Random.value < 0.5f)
        {
            steering += Seek(nearestTrash.transform.position);
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Trash>() != null)
        {
            touchTimer += Time.deltaTime;
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
            touchTimer = 0f; // reset if no longer touching
        }
    }
}

