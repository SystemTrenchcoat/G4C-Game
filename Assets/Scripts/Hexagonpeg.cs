using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagonpeg : MonoBehaviour
{
    public GameObject diamondPeg;//assign a DP for direction change

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("chip"))
        {
            Debug.Log("Hexagon Peg collided with Chip!");

            if (diamondPeg != null)
            {
                Diamondpeg diamondScript = diamondPeg.GetComponent<Diamondpeg>();
                if (diamondScript != null)
                {
                    diamondScript.ToggleRotationDirection();//call diamond function for toggle direction
                }
                else
                {
                    Debug.LogWarning("DP script not found on assigned GameObject");
                }
            }
            else
            {
                Debug.LogWarning("DP reference missing in Hexagon Peg Inspector");
            }
        }
    }

}
