using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamondpeg : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private int direction = 1; //closewise for 1

    private void Update()
    {
        //2D rotation
        transform.Rotate(0, 0, direction * rotationSpeed * Time.deltaTime);
    }

    public void ToggleRotationDirection()
    {
        direction *= -1; //flip direction by multiply -1

        if (direction == 1)
        {
            Debug.Log("Diamond Peg is now spinning CLOCKWISE");
        }
        else
        {
            Debug.Log("Diamond Peg is now spinning ANTI-CLOCKWISE");
        }
    }

}
