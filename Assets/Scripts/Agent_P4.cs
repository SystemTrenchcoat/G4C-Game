using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoidAgent_P4 : MonoBehaviour
{
    //physics
    public float maxSpeed = 5f;
    public float maxForce = 10f;
    public float mass = 1f;

    //neighbour
    public float neighborRadius = 3f;
    public LayerMask agentLayer;

    //weights on behaviors
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;

    //movement
    protected Vector2 velocity;
    protected Vector2 acceleration;

    void Update()
    {
        ClampPositionToScreen();
        Vector2 steeringForce = CalculatedSteering();
        ApplyForce(steeringForce);

        velocity += acceleration * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        transform.position += (Vector3)velocity * Time.deltaTime;
        acceleration = Vector2.zero;

        //rotate to face movement
        if (velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    protected void ApplyForce(Vector2 force)
    {
        acceleration += force / mass;
    }

    protected void ClampPositionToScreen()
    {
        Vector3 pos = transform.position;
        Vector3 viewPos = Camera.main.WorldToViewportPoint(pos);

        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);

        pos = Camera.main.ViewportToWorldPoint(viewPos);
        transform.position = pos;
    }


    //========== Steering Behaviors ==========

    //move towards target
    protected Vector2 Seek(Vector2 target)
    {
        Vector2 desired = (target - (Vector2)transform.position).normalized * maxSpeed;
        return desired - velocity;
    }

    //move towards target but slows down before arrival
    protected Vector2 Arrive(Vector2 target, float slowingDistance = 3f)
    {
        Vector2 toTarget = target - (Vector2)transform.position;
        float distance = toTarget.magnitude;
        float rampedSpeed = maxSpeed * (distance / slowingDistance);
        float clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed);
        Vector2 desired = (clippedSpeed / distance) * toTarget;
        return desired - velocity;
    }

    //push away from target to avoid overlapping
    protected Vector2 Separation(List<BoidAgent_P4> neighbors)
    {
        Vector2 steer = Vector2.zero;
        int count = 0;

        foreach (BoidAgent_P4 neighbor in neighbors)
        {
            if (neighbor == this) continue;

            float dist = Vector2.Distance(transform.position, neighbor.transform.position);
            if (dist < neighborRadius && dist > 0)
            {
                Vector2 diff = ((Vector2)transform.position - (Vector2)neighbor.transform.position).normalized;
                diff = diff.normalized / dist;
                steer += diff;
                count++;
            }
        }

        if (count > 0)
        {
            steer /= count;
            steer = steer.normalized * maxSpeed;
            steer -= velocity;
            steer = Vector2.ClampMagnitude(steer, maxForce);
        }

        return steer;
    }

    //makes the agent match the average direction of nearby agents
    protected Vector2 Alignment(List<BoidAgent_P4> neighbors)
    {
        Vector2 avgVelocity = Vector2.zero;
        int count = 0;

        foreach (BoidAgent_P4 neighbor in neighbors)
        {
            if (neighbor == this) continue;

            float dist = Vector2.Distance(transform.position, neighbor.transform.position);
            if (dist < neighborRadius)
            {
                avgVelocity += neighbor.GetVelocity();
                count++;
            }
        }

        if (count > 0)
        {
            avgVelocity /= count;
            avgVelocity = avgVelocity.normalized * maxSpeed;
            return avgVelocity - velocity;
        }

        return Vector2.zero;
    }

    //makes the agent move toward the average position of neighbors
    protected Vector2 Cohesion(List<BoidAgent_P4> neighbors)
    {
        Vector2 center = Vector2.zero;
        int count = 0;

        foreach (BoidAgent_P4 neighbor in neighbors)
        {
            if (neighbor == this) continue;

            float dist = Vector2.Distance(transform.position, neighbor.transform.position);
            if (dist < neighborRadius)
            {
                center += (Vector2)neighbor.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            center /= count;
            return Seek(center);
        }

        return Vector2.zero;
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    protected List<BoidAgent_P4> GetNeighbors()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, neighborRadius, agentLayer);
        List<BoidAgent_P4> neighbors = new List<BoidAgent_P4>();

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            BoidAgent_P4 agent = hit.GetComponent<BoidAgent_P4>();

            if (agent != null && agent != this)
            {
                neighbors.Add(agent);
            }
        }

        return neighbors;   
    }


    protected abstract Vector2 CalculatedSteering();
}
