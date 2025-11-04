using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLooker : MonoBehaviour
{
    public Vector2 MousePos;

    void Update()
    {
        GetMousePosition();
        if (Input.GetMouseButtonDown(0)) 
        {

        }
        Look();
    }

    void GetMousePosition()
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Look()
    {
        //calculation to find the direction of the mouse
        Vector2 direction = MousePos - (Vector2)transform.position;

        //find angle from direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //rotate the object from the angle
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
