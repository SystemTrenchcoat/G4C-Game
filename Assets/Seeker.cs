using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : Agent
{
    public Transform target;

    protected override Vector2 CalculatedSteering()
    {
        if (target == null) return Vector2.zero;
        return Seek(target.position);
    }

    //show visual for debugging
    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}