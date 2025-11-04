using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fruit : MonoBehaviour
{
    public int points = 1; // Points awarded when sliced
    public bool isSpecialFruit = false; // Set this to true for Fruit 4
    public bool isMultiplierFruit = false; // Set this to true for the multiplier fruit
    public GameObject noSliceZonePrefab;

    private int noSliceZoneCount = 0; // Counter for no-slice zones

    public bool CanBeSliced()
    {
        return noSliceZoneCount == 0; // Can only be sliced if not inside any no-slice zone
    }

    public void EnterNoSliceZone()
    {
        noSliceZoneCount++;
        Debug.Log("Entered no-slice zone. Count: " + noSliceZoneCount);
    }

    public void ExitNoSliceZone()
    {
        noSliceZoneCount--;
        Debug.Log("Exited no-slice zone. Count: " + noSliceZoneCount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("blade") && CanBeSliced()) // Ensure slicing is enabled
        {
            if (isMultiplierFruit)
            {
                //times current score by 2
                ScoreManager.Instance.MultiplyScore(2);
                Debug.Log("Score multiplied by 2");
            }
            else if (!isSpecialFruit)
            {
                //give points
                ScoreManager.Instance.AddPoints(points);
                Debug.Log("add points");
            }
            else
            {
                //cause smoke screen
                CreateNoSliceZone();
                Debug.Log("spawn cloud");
            }

            Destroy(gameObject); //"Slice" the fruit -> destroying it
        }
    }

    public void CreateNoSliceZone()
    {
        if (noSliceZonePrefab != null)
        {
            Debug.Log("NoSliceZone prefab assigned in the Inspector");
            Instantiate(noSliceZonePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("NoSliceZone prefab not assigned in the Inspector");
        }
    }
}