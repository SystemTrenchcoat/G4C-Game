using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent_P4 : MonoBehaviour
{
    [Header("Agent Settings")]
    public float maxSpeed = 5f;
    public float maxForce = 10f;
    public float mass = 1f;

    [Header("Flocking Settings")]
    public float neighborRadius = 3f;
    public float separationRadius = 1.5f;

    protected Vector2 velocity;
    protected Vector2 acceleration;

    protected virtual void Update()
    {
        Vector2 steeringForce = CalculatedSteering();
        ApplyForce(steeringForce);

        velocity += acceleration * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        transform.position += (Vector3)velocity * Time.deltaTime;
        acceleration = Vector2.zero;

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

    // SEEK: Move directly toward a target
    protected Vector2 Seek(Vector2 targetPosition)
    {
        Vector2 desiredVelocity = (targetPosition - (Vector2)transform.position).normalized * maxSpeed;
        return desiredVelocity - velocity;
    }

    // ARRIVE: Move toward a target and slow down as it gets closer
    protected Vector2 Arrive(Vector2 targetPosition, float slowingDistance = 5f)
    {
        Vector2 toTarget = targetPosition - (Vector2)transform.position;
        float distance = toTarget.magnitude;

        float rampedSpeed = maxSpeed * (distance / slowingDistance);
        float clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed);

        Vector2 desiredVelocity = (clippedSpeed / distance) * toTarget;
        return desiredVelocity - velocity;
    }

    // SEPARATION: Steer away from nearby agents to avoid crowding
    protected Vector2 Separation(List<Agent> neighbors)
    {
        Vector2 force = Vector2.zero;
        int count = 0;

        foreach (Agent neighbor in neighbors)
        {
            if (neighbor == this) continue;

            float dist = Vector2.Distance(transform.position, neighbor.transform.position);
            if (dist < separationRadius && dist > 0)
            {
                Vector2 away = (Vector2)(transform.position - neighbor.transform.position);
                force += away.normalized / dist;
                count++;
            }
        }

        return count > 0 ? force.normalized * maxForce : Vector2.zero;
    }

    // ALIGNMENT: Match velocity with nearby agents (fixed access to velocity)
    protected Vector2 Alignment(List<Agent> neighbors)
    {
        Vector2 avgVelocity = Vector2.zero;
        int count = 0;

        foreach (Agent neighbor in neighbors)
        {
            if (neighbor == this) continue;
            if (Vector2.Distance(transform.position, neighbor.transform.position) < neighborRadius)
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

    // COHESION: Move toward the average position of nearby agents
    protected Vector2 Cohesion(List<Agent> neighbors)
    {
        Vector2 center = Vector2.zero;
        int count = 0;

        foreach (Agent neighbor in neighbors)
        {
            if (neighbor == this) continue;
            if (Vector2.Distance(transform.position, neighbor.transform.position) < neighborRadius)
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

    // Public getter for velocity so other agents can access it safely
    public Vector2 GetVelocity()
    {
        return velocity;
    }

    // Must be implemented by subclasses (Piranha, Alpha, Beta)
    protected abstract Vector2 CalculatedSteering();
}


