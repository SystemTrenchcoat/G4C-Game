using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arriver : Agent
{
    public Transform target;
    public float arrivalDistance = 5f;

    protected override Vector2 CalculatedSteering()
    {
        if (target == null) return Vector2.zero;
        return Arrive(target.position, arrivalDistance);
    }

    //show visual for debugging
    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, target.position);

            // Show arrival radius
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target.position, arrivalDistance);
        }
    }
}