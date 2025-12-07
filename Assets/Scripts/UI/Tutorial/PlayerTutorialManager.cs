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
    //private float sonarTimer = 0;
    //private float releaseTimer = 0;
    private float trashTimer = 0;

    //Text Variables
    public TextMeshProUGUI tutorText;
    [Range(0f, 1f)] public float textOpacity = 1f;

    //Tutorial Checks
    private bool moveTut = true;
    private bool sonarTut = true;
    private bool releaseTut = true;
    private bool trashTut = true;
    private bool trashInfo = true;

    // Start is called before the first frame update
    void Start()
    {
        currentPos = transform.position;
        previousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debugging Start
        Debug.Log("Move: " + moveTut);
        Debug.Log("Sonar: " + sonarTut);
        Debug.Log("Release: " + releaseTut);
        Debug.Log("Trash: " + trashTut);
        Debug.Log("Trash Info: " + trashInfo);
        //Debugging End

        //Move Tutorial Start
        previousPos = currentPos;
        currentPos = transform.position;

        if (currentPos != previousPos)
        {
            EmptyText();
            moveTut = false;
        }

        else
            moveTimer += Time.deltaTime;

        if (moveTut && moveTimer >= tutDelay)
            ShowText("Move your mouse to steer the boat.");
        //Move Tutorial End

        //Release Tutorial Start
        if (releaseTut && gameObject.transform.parent.GetComponent<Boat>().releasedFish){
            EmptyText();
            releaseTut = false;
        }
        //Release Tutorial End;

        //Trash Tutorial Start
        if(!trashTut && trashInfo)
        {
            ShowText("According to a 2023 research paper, much of the plastic in the coral reefs is the result of fishing");
            trashTimer += Time.deltaTime;

            if(trashTimer >= tutDelay)
            {
                EmptyText();
                trashInfo = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Sonar Tutorial Start
        if (sonarTut)
        {
            Fish fish = collision.gameObject.GetComponent<Fish>();
            if (fish != null) return;

            ShowText("Left-click to scare the fish away with sonar.");
        }


        if (sonarTut)
        {
            SoundWave sonar = collision.gameObject.GetComponent<SoundWave>();
            if (sonar != null) return;

            EmptyText();
            sonarTut = false;
        }
        //Sonar Tutorial End

        //Release Tutorial Start
        if (releaseTut && gameObject.transform.parent.GetComponentInChildren<Fish>())
            ShowText("Fish can be caught if the boat touches them. Alternate A and D keys to release the fish.");
        //Release Tutorial End;

        //Trash Tutorial Start
        if (trashTut)
        {
            Trash trash = collision.gameObject.GetComponent<Trash>();
            if (trash != null) return;

            ShowText("Sail over the trash for a few moments to collect it");
        }
        //Trash Tutorial End
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Trash Tutorial Start
        if (trashTut)
        {
            Trash trash = collision.gameObject.GetComponent<Trash>();
            if (trash != null) return;

            EmptyText();
            trashTut = false;
        }
        //Trash Tutorial End
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

    private void EmptyText()
    {
        ShowText("");
    }
}
