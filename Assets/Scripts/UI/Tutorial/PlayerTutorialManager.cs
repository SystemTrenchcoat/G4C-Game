using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    //Move Tutorial Variables
    private Vector2 currentPos;
    private Vector2 previousPos;

    //Timers
    private float tutDelay = 2.4f;
    private float moveTimer = 0;
    private float sonarTimer = 0;
    private float releaseTimer = 0;
    private float trashTimer = 0;

    //Text Variables
    public TextMeshProUGUI tutorText;
    [Range(0f, 1f)] public float textOpacity = 1f;

    //Tutorial Checks
    private bool moveTut = true;
    private bool sonarTut = true;
    private bool releaseTut = true;
    private bool trashTut = true;

    // Start is called before the first frame update
    void Start()
    {
        currentPos = transform.position;
        previousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Move Tutorial Start
        if (currentPos != previousPos)
            moveTut = false;

        else
            moveTimer += Time.deltaTime;

        if (moveTimer >= tutDelay && moveTut)
        {
            ShowText("Move your mouse to steer the boat.");
        }
        //Move Tutorial End


    }

    private void ShowText(string text)
    {
        if (tutorText == null) return;

        tutorText.text = text;

        Color c = tutorText.color;
        c.a = textOpacity;
        tutorText.color = c;

        tutorText.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fish fish = collision.gameObject.GetComponent<Fish>();
        if (sonarTut)
        {
            if (fish != null)   return;

        }
    }
}
