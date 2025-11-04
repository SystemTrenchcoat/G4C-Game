using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSliceZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Notify the fruit that it entered a no-slice zone
        fruit fruit = other.GetComponent<fruit>();
        if (fruit != null)
        {
            fruit.EnterNoSliceZone();
            Debug.Log("Fruit entered no-slice zone: " + other.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Notify the fruit that it exited a no-slice zone
        fruit fruit = other.GetComponent<fruit>();
        if (fruit != null)
        {
            fruit.ExitNoSliceZone();
            Debug.Log("Fruit exited no-slice zone: " + other.name);
        }
    }
}