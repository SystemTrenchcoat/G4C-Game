using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : BoidAgent_P4
{
    protected override Vector2 CalculatedSteering()
    {
        // Just wander around
        return Wander();
    }

    private Vector2 Wander()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
        Vector2 targetPos = (Vector2)transform.position + randomOffset;
        return Seek(targetPos);
    }
}
