using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : BoidAgent_P4
{
    public GameObject soundWavePrefab;

    protected override Vector2 CalculatedSteering()
    {
        // Always seek mouse position tightly
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 steering = Seek(mousePos) * 2.0f; // stronger seek
        return Vector2.ClampMagnitude(steering, maxForce);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Instantiate(soundWavePrefab, transform.position, Quaternion.identity);
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

