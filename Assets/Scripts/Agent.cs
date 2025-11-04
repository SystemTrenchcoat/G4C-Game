using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    //declare variables
    public float maxSpeed = 5f;
    public float maxForce = 10f;
    public float mass = 1f;

    protected Vector2 velocity;
    protected Vector2 acceleration;

    void Update()
    {
        //calculate steering force
        Vector2 steeringForce = CalculatedSteering();

        //apply force to acceleration
        ApplyForce(steeringForce);

        //update velocity and position
        velocity += acceleration * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        transform.position += (Vector3)velocity * Time.deltaTime;

        //reset acceleration
        acceleration = Vector2.zero;

        //rotate to face direction of movement
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

    //seek behavior: moves toward target at max speed
    protected Vector2 Seek(Vector2 targetPosition)
    {
        Vector2 desiredVelocity = (targetPosition - (Vector2)transform.position).normalized * maxSpeed;
        return desiredVelocity - velocity;
    }

    //arrive behavior: slows down as it approaches target
    protected Vector2 Arrive(Vector2 targetPosition, float slowingDistance)
    {
        Vector2 toTarget = targetPosition - (Vector2)transform.position;
        float distance = toTarget.magnitude;

        //calculate ramped speed
        float rampedSpeed = maxSpeed * (distance / slowingDistance);
        float clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed);

        Vector2 desiredVelocity = (clippedSpeed / distance) * toTarget;
        return desiredVelocity - velocity;
    }

    //overload for Arrive with default slowing distance
    protected Vector2 Arrive(Vector2 targetPosition)
    {
        return Arrive(targetPosition, 5f);
    }

    //abstract method to be implemented by subclasses
    protected abstract Vector2 CalculatedSteering();

    internal Vector2 GetVelocity()
    {
        throw new NotImplementedException();
    }
}