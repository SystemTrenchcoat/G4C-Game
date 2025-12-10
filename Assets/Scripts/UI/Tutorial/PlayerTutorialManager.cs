using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    //Tutorial Type Enum
    private enum tutType { move, sonar, release, trash, trashInfo };

    //Move Tutorial Variables
    private Vector2 currentPos;
    private Vector2 previousPos;

    //Timers
    private const float tutDelay = 2.4f;
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
    private bool trashInfo = true;
    private bool tutShowing = false;

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
        //Debug.Log("Move: " + moveTut);
        //Debug.Log("Sonar: " + sonarTut);
        Debug.Log("Release: " + releaseTut);
        Debug.Log("Trash: " + trashTut);
        //Debug.Log("Trash Info: " + trashInfo);
        //Debugging End

        //Move Tutorial Start
        previousPos = currentPos;
        currentPos = transform.position;

        if (Vector2.Distance(currentPos, previousPos) >= .02f && moveTut)
        {
            //Debug.Break();
            EmptyText();
            moveTut = false;
        }

        else
            moveTimer += Time.deltaTime;

        if (moveTut && moveTimer >= tutDelay && !tutShowing)
            ShowTutorial(tutType.move);
        //Move Tutorial End

        if(!sonarTut && tutShowing)
        {
            sonarTimer += Time.deltaTime;
            ShowTutorial(tutType.sonar);

            if (sonarTimer >= tutDelay)
                EmptyText();
        }

        //Release Tutorial Start
        if (releaseTimer >= tutDelay && gameObject.transform.parent.GetComponent<Boat>().releasedFish && tutShowing)
        {
            Debug.Log(gameObject.transform.parent.GetComponent<Boat>().releasedFish);
            EmptyText();
        }
        else if (!releaseTut)
        {
            releaseTimer += Time.deltaTime;
            ShowTutorial(tutType.release);
        }
        //Release Tutorial End;

        //Trash Tutorial Start
        if (trashTimer >= tutDelay)
        {
            EmptyText();

            if (trashInfo && !tutShowing)
            {
                ShowTutorial(tutType.trashInfo);
                trashTimer += Time.deltaTime;

                if (trashTimer >= tutDelay * 2)
                {
                    //Debug.Break();
                    EmptyText();
                    trashInfo = false;
                }
            }
        }
        else if (!trashTut)
        {
            trashTimer += Time.deltaTime;
            Debug.Log(trashTimer);
            ShowTutorial(tutType.trash);
        }
        //Trash Tutorial End
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);

        //Sonar Tutorial Start
        if (sonarTut && !tutShowing)
        {
            Fish fish = collision.gameObject.GetComponent<Fish>();
            if (fish != null)
            {
                ShowTutorial(tutType.sonar);
                sonarTut = false;
            }
        }
        //Sonar Tutorial End

        //Release Tutorial Start
        if (releaseTut && gameObject.transform.parent.GetComponentInChildren<Boat>().fishCaught && !tutShowing)
        {
            ShowTutorial(tutType.release);
            releaseTut = false;
        }
        //Release Tutorial End;

        //Trash Tutorial Start
        if (trashTut && !tutShowing)
        {
            Trash trash = collision.gameObject.GetComponent<Trash>();
            if (trash != null)
            {
                Debug.Log("Trash Tut Display! - " + collision);
                //Debug.Break();
                ShowTutorial(tutType.trash);
                trashTut = false;
            }
        }
        //Trash Tutorial End
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    //Trash Tutorial Start
    //    if (trashTut)
    //    {
    //        Trash trash = collision.gameObject.GetComponent<Trash>();
    //        if (trash == null) return;

    //        //Debug.Break();
    //        EmptyText();
    //        trashTut = false;
    //    }
    //    //Trash Tutorial End
    //}

    private void ShowText(string text)
    {
        if (tutorText == null) return;

        tutorText.text = text;

        Color c = tutorText.color;
        c.a = textOpacity;
        tutorText.color = c;

        tutorText.gameObject.SetActive(true);

        tutShowing = true;

        Debug.Log("Text - " + text);
        Debug.Log("Tut text - " + tutorText.text);
    }

    private void EmptyText()
    {
        //ShowText("");
        tutShowing = false;
    }

    private void ShowTutorial(tutType type)
    {
        switch (type)
        {
            case tutType.move:
                ShowText("Move your mouse to steer the boat.");
                break;
            case tutType.sonar:
                ShowText("Left-click to scare the fish away with sonar.");
                break;
            case tutType.release:
                ShowText("Fish are caught if the boat touches them. Alternate A and D keys to release the fish.");
                break;
            case tutType.trash:
                ShowText("Sail over the trash for a few moments to collect it");
                break;
            case tutType.trashInfo:
                ShowText("According to a 2023 research paper, much of the plastic in the coral reefs is the result of fishing");
                break;
        }
    }
}
