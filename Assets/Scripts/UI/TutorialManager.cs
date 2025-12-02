using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("UI")]
    public TextMeshProUGUI tutorText;
    [Range(0f, 1f)] public float textOpacity = 1f;

    [Header("References")]
    public Boat boat;
    public GameObject fishPrefab;
    public Transform fishSpawnPoint;
    public GameObject trashPrefab;
    public Transform trashSpawnPoint;

    [Header("Movement Detection Settings")]
    public float movementThresholdPerSecond = 0.5f;
    public float movementSampleWindow = 5f;

    private Vector3 lastBoatPos;
    private float accumDistance = 0f;
    private float accumTime = 0f;

    private int step = 0;
    private bool stepActive = false;
    public int CurrentStep => step;

    // Tutorial timing
    public float stepAdvanceDelay = 2.4f;

    // Tutorial fish
    private Fish tutorialFish;
    private bool fishHitBySonar = false;
    private float fishAdvanceTimer = 0f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (tutorText != null) tutorText.gameObject.SetActive(false);
        if (boat != null) lastBoatPos = boat.transform.position;
        if (GameManager.instance != null && GameManager.instance.tutorialEnabled == false)
            return;
        BeginTutorial();
    }

    private void Update()
    {
        if (!stepActive) return;

        if (tutorText != null)
            tutorText.gameObject.SetActive(true);

        ApplyTextOpacity();

        switch (step)
        {
            case 1:
                CheckMovementProgress();
                break;

            case 2:
                if (fishHitBySonar)
                {
                    fishAdvanceTimer += Time.deltaTime;
                    if (fishAdvanceTimer >= stepAdvanceDelay)
                    {
                        stepActive = false;
                        ShowStep_3_ReleaseFish();
                    }
                }
                break;
        }
    }

    // ---------------- Tutorial Control ----------------

    public void BeginTutorial()
    {
        step = 0;
        stepActive = false;
        ShowStep_1_Movement();
    }

    public void ForceEndTutorial()
    {
        stepActive = false;

        if (tutorText != null)
            tutorText.gameObject.SetActive(false);

        if (tutorialFish != null)
            Destroy(tutorialFish.gameObject);
    }

    public void DisableTutorText()
    {
        if (tutorText != null)
            tutorText.gameObject.SetActive(false);
    }

    // ---------------- Tutorial Steps ----------------

    private void ShowStep_1_Movement()
    {
        step = 1;
        stepActive = true;
        accumDistance = 0f;
        accumTime = 0f;

        ShowText("Move your mouse to steer the boat.");
    }

    private void CheckMovementProgress()
    {
        if (boat == null) return;

        Vector3 currentPos = boat.transform.position;
        float frameDistance = Vector3.Distance(currentPos, lastBoatPos);
        lastBoatPos = currentPos;

        accumDistance += frameDistance;
        accumTime += Time.deltaTime;

        if (accumTime >= movementSampleWindow)
        {
            float avgSpeed = accumDistance / accumTime;

            if (avgSpeed >= movementThresholdPerSecond)
            {
                stepActive = false;

                // Step-specific success message
                ShowText("Great! You moved the boat!");

                StartCoroutine(AdvanceAfterDelay(ShowStep_2_Sonar));
            }

            accumDistance = 0f;
            accumTime = 0f;
        }
    }

    private void ShowStep_2_Sonar()
    {
        step = 2;
        stepActive = true;
        fishHitBySonar = false;
        fishAdvanceTimer = 0f;

        ShowText("Left-click to hit the fish with sonar.");

        if (fishPrefab != null)
        {
            GameObject go = Instantiate(fishPrefab, fishSpawnPoint.position, Quaternion.identity);
            tutorialFish = go.GetComponent<Fish>();
        }
    }

    private void ShowStep_3_ReleaseFish()
    {
        step = 3;
        stepActive = true;
        ShowText("Fish can be caught if the boat touches them. Alternate A and D keys to release the fish.");
    }

    public void OnFishReleased_Tutorial()
    {
        if (step != 3) return;

        stepActive = false;

        // Step-specific success message
        ShowText("Good job releasing the fish!");

        StartCoroutine(AdvanceAfterDelay(ShowStep_4_Trash));
    }

    private void ShowStep_4_Trash()
    {
        step = 4;
        stepActive = true;
        ShowText("Collect floating trash.");

        if (trashPrefab != null)
            Instantiate(trashPrefab, trashSpawnPoint.position, Quaternion.identity);
    }

    public void OnTrashCollected_Tutorial()
    {
        if (step != 4) return;

        stepActive = false;

        // FINAL COMPLETION MESSAGE (persistent)
        ShowText("Tutorial complete! Press SPACE to begin.");

        // Mark tutorial as finished
        if (GameManager.instance != null)
            GameManager.instance.tutorialEnabled = false;
    }

    // ---------------- Fish-Sonar Integration ----------------

    public bool IsTutorialFish(Fish fish)
    {
        return fish == tutorialFish;
    }

    public void OnTutorialFishHitBySonar(Fish fish)
    {
        if (step != 2 || fish != tutorialFish) return;

        fishHitBySonar = true;
        fishAdvanceTimer = 0f;

        // Step-specific success message
        ShowText("Nice hit! You used the sonar correctly.");
    }

    // ---------------- Text Helpers ----------------

    private void ShowText(string text)
    {
        if (tutorText == null) return;

        tutorText.text = text;

        Color c = tutorText.color;
        c.a = textOpacity;
        tutorText.color = c;

        tutorText.gameObject.SetActive(true);
    }

    private void ApplyTextOpacity()
    {
        if (tutorText == null) return;

        Color c = tutorText.color;
        c.a = textOpacity;
        tutorText.color = c;
    }

    // ---------------- Utility ----------------

    private IEnumerator AdvanceAfterDelay(System.Action nextStep)
    {
        yield return new WaitForSeconds(stepAdvanceDelay);
        nextStep?.Invoke();
    }
}








