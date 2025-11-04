using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2FN_sliceDetect : MonoBehaviour
{
    Vector2 lastMousePos;
    public LayerMask fruitLayer;
    public float minSliceDistance = 0.5f; // Minimum distance to consider it a slice
    public float minSliceTime = 0.2f; // Minimum time to consider it a slice
    private float mouseDownTime;

    void Update()
    {
        if (!GameStateManager.Instance.IsDuringGame()) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Record the time on mouse left pressed
            mouseDownTime = Time.time;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (lastMousePos != Vector2.zero)
            {
                float distance = Vector2.Distance(lastMousePos, currentMousePos);

                // Check if the mouse has been held for the minimum time and dragged the minimum distance
                if (Time.time - mouseDownTime >= minSliceTime && distance >= minSliceDistance)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(lastMousePos, (currentMousePos - lastMousePos),
                                                                distance, fruitLayer);

                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null)
                        {
                            Debug.Log("intercept/ HIT!/ SLICED!");

                            // Check if the fruit can be sliced
                            fruit fruit = hit.collider.GetComponent<fruit>();
                            if (fruit != null && fruit.CanBeSliced()) // Check if the fruit is not inside any no-slice zone
                            {
                                if (fruit.isMultiplierFruit)
                                {
                                    //times current score by 2
                                    ScoreManager.Instance.MultiplyScore(2);
                                    Debug.Log("Score multiplied by 2");
                                }
                                else if (!fruit.isSpecialFruit)
                                {
                                    //add points for normal fruits
                                    if (ScoreManager.Instance != null)
                                    {
                                        ScoreManager.Instance.AddPoints(fruit.points);
                                        Debug.Log("add points");
                                    }
                                    else
                                    {
                                        Debug.LogError("ScoreManager.Instance is null");
                                    }
                                }
                                else
                                {
                                    //smoke bomb for Fruit 4
                                    fruit.CreateNoSliceZone();
                                    Debug.Log("spawn cloud");
                                }

                                Destroy(hit.collider.gameObject); // Fruit is sliced -> destroying it
                            }
                            else
                            {
                                Debug.Log("Fruit cannot be sliced (inside no-slice zone)");
                            }
                        }
                    }

                    Debug.DrawLine(lastMousePos, currentMousePos, Color.red, 0.1f);
                }
            }

            lastMousePos = currentMousePos;
        }
        else
        {
            lastMousePos = Vector2.zero;
        }
    }
}