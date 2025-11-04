using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    public Vector2 MousePos;

    void Update()
    {
        GetMousePosition();
        Move();
    }

    void GetMousePosition()
    {
        // Convert mouse position to world space
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Move()
    {
        transform.position = MousePos;
    }
}
