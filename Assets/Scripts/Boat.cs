using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : BoidAgent_P4
{
    public GameObject soundWavePrefab;
    [SerializeField] private float followStrength = 8f; // higher = tighter follow
    [SerializeField] private float damping = 5f;        // higher = less slippery
    [SerializeField] private float soundWaveCooldown = 1f;
    private float nextSoundWaveTime = 0f;

    protected override Vector2 CalculatedSteering()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 desiredVelocity = (mousePos - (Vector2)transform.position).normalized * maxSpeed;
        Vector2 steering = desiredVelocity - velocity; // base steering from BoidAgent
        steering *= followStrength;

        // Apply damping to make boat stop sliding
        velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * damping);

        return Vector2.ClampMagnitude(steering, maxForce);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= nextSoundWaveTime)
        {
            Instantiate(soundWavePrefab, transform.position, Quaternion.identity);
            nextSoundWaveTime = Time.time + soundWaveCooldown; // start cooldown
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Fish>() != null)
        {
            Destroy(collision.gameObject); // instantly remove betta
        }
        else if (collision.GetComponent<Trash>() != null)
        {
            Destroy(collision.gameObject); // instantly remove trash
        }
    }
}

