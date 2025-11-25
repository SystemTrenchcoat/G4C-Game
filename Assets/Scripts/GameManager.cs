using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Start, Play, LevelEnd, End }
    public GameState currentState = GameState.Start;

    [Header("Main UI")]
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI restartText;
    public TextMeshProUGUI countdownText;
    public GameObject backgroundImage;

    [Header("Mash UI")]
    public TextMeshProUGUI caughtText;
    public TextMeshProUGUI mashKeyLeft;
    public TextMeshProUGUI mashKeyRight;

    [Header("Level Settings")]
    public float levelTimeLimit = 90f; // Changed: fixed 90 sec
    public int currentLevel = 1;

    [Header("References")]
    public GameObject trashPrefab;
    public BettaSpawner spawner;
    public GameObject cooldownBarUI;

    public TrashSpawner trashSpawner; // Changed: new reference to TrashSpawner

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    private float totalScore = 0f;         // Changed
    private float levelScore = 0f;         // Changed
    private float pendingFishPenalty = 0f; // Changed
    private int trashCountThisLevel = 0;   // Changed

    private float levelTimer = 0f;
    private bool levelActive = false;
    private Boat boat;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        boat = FindObjectOfType<Boat>();

        Time.timeScale = 0f;
        ShowStartMessage();

        if (spawner != null)
            spawner.enabled = false;

        if (cooldownBarUI != null)
            cooldownBarUI.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        HideMashUI();
        UpdateScoreUI();
    }

    private void Update()
    {
        UpdateMashUI();

        switch (currentState)
        {
            case GameState.Start:
                if (Input.GetKeyDown(KeyCode.Space))
                    StartLevel(1);
                break;

            case GameState.Play:
                HandleLevelTimer();
                CheckTrashRemaining();
                break;

            case GameState.LevelEnd:
                if (Input.GetKeyDown(KeyCode.Space))
                    StartNextLevel();

                if (Input.GetKeyDown(KeyCode.Escape)) // Changed
                    EndRun();
                break;

            case GameState.End:
                if (Input.GetKeyDown(KeyCode.Space))
                    RestartGame();
                break;
        }
    }

    private void ShowStartMessage()
    {
        if (infoText != null)
        {
            infoText.gameObject.SetActive(true);
            infoText.text =
                "ReefTender - Protect What Remains\n\n" +
                "The reef you grew up beside is struggling. As part of a local restoration\n" +
                "group, you patrol these waters to collect waste and safeguard the sea life\n" +
                "that still thrives here. Every piece of trash you remove brings the reef\n" +
                "one step closer to recovery.\n\n" +
                "CONTROLS\n" +
                "Move with the mouse.\n" +
                "Left-click to send out a pulse to steer fish away - fish will flash red and dies when touching trash\n" +
                "Stay contact with trash to pick it up - trash will flash yellow during pick up.\n" +
                "Touching fish captures them - this slows your boat and costs points if not released.\n" +
                "Tap A and D rapidly to release captured fish.\n" +
                "Press SPACE to start the mission.";
        }

        if (restartText != null)
            restartText.gameObject.SetActive(false);

        if (backgroundImage != null)
            backgroundImage.SetActive(true);
    }

    // ---------------- LEVEL CONTROL ----------------
    private void StartLevel(int level)
    {
        currentLevel = level;
        currentState = GameState.Play;
        Time.timeScale = 1f;

        if (infoText != null) infoText.gameObject.SetActive(false);
        if (backgroundImage != null) backgroundImage.SetActive(false);

        levelTimer = levelTimeLimit;
        levelActive = true;

        if (cooldownBarUI != null)
            cooldownBarUI.SetActive(true);

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        if (spawner != null)
        {
            spawner.enabled = true;
            spawner.maxFishCount = 20;
        }

        // Remove all fish from previous level   // Changed
        Fish[] allFish = FindObjectsOfType<Fish>();
        foreach (Fish f in allFish) Destroy(f.gameObject);

        // Remove all trash from previous level   // Changed
        if (trashSpawner != null)
            trashSpawner.ClearAllTrash();

        // Calculate trash count for level
        trashCountThisLevel = GetTrashCountForLevel(level); // Changed

        // Spawn trash using TrashSpawner
        if (trashSpawner != null) // Changed
            trashSpawner.SpawnTrash(trashCountThisLevel);

        // recenter boat
        if (boat != null && Camera.main != null)
        {
            boat.transform.position = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                0f
            );
        }

        // Reset level score
        levelScore = 0f;         // Changed
        pendingFishPenalty = 0f; // Changed
        UpdateScoreUI();
    }

    private void HandleLevelTimer()
    {
        if (!levelActive) return;

        levelTimer -= Time.deltaTime;

        if (countdownText != null)
            countdownText.text = Mathf.CeilToInt(levelTimer) + "s";

        if (levelTimer <= 0f)
        {
            levelTimer = 0f;
            EndLevel();
        }
    }

    private void EndLevel()
    {
        levelActive = false;
        Time.timeScale = 0f;

        if (spawner != null)
            spawner.enabled = false;

        if (cooldownBarUI != null)
            cooldownBarUI.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        HideMashUI();

        // Apply fish penalties   // Changed
        ApplyFishPenalty();
        totalScore += levelScore; // Changed

        // Destroy fish   // Changed
        Fish[] allFish = FindObjectsOfType<Fish>();
        foreach (Fish f in allFish) Destroy(f.gameObject);

        // Destroy trash    // Changed
        if (trashSpawner != null)
            trashSpawner.ClearAllTrash();

        currentState = GameState.LevelEnd;

        if (infoText != null)
        {
            infoText.gameObject.SetActive(true);
            infoText.text =
                "#" + currentLevel + " levels completed\n" +  // Changed
                "Total Score: " + totalScore.ToString("F2") + "\n" +
                "Press SPACE for next level or ESC to end run";
        }

        if (backgroundImage != null)
            backgroundImage.SetActive(true);
    }

    private void StartNextLevel()
    {
        StartLevel(currentLevel + 1); // Changed: infinite levels
    }

    // ---------------- END RUN ----------------
    private void EndRun()
    {
        currentState = GameState.End;
        Time.timeScale = 0f;

        if (infoText != null)
            infoText.gameObject.SetActive(false); // Changed: hide LevelEnd text

        if (restartText != null)
        {
            restartText.gameObject.SetActive(true);
            restartText.text =
                "#" + currentLevel + " levels completed\n" + // Changed
                "Total Score: " + totalScore.ToString("F2") + "\n" +
                "Press SPACE to restart";
        }

        if (backgroundImage != null)
            backgroundImage.SetActive(true);

        if (cooldownBarUI != null)
            cooldownBarUI.SetActive(false);

        HideMashUI();
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ---------------- TRASH LOGIC ----------------

    private void CheckTrashRemaining()
    {
        Trash[] allTrash = FindObjectsOfType<Trash>();
        if (allTrash.Length == 0)
            EndLevel();
    }

    private int GetTrashCountForLevel(int level) // Changed: infinite pattern
    {
        if (level <= 0) return 0;

        int group = (level - 1) / 3;
        int position = (level - 1) % 3;

        return 3 + group * 4 + position;
    }

    // ---------------- MASH UI ----------------

    private void UpdateMashUI()
    {
        if (boat == null)
            return;

        int count = boat.GetCaughtFishCount();

        if (count == 0)
        {
            HideMashUI();
            return;
        }

        if (caughtText != null)
        {
            caughtText.gameObject.SetActive(true);
            caughtText.text = "Fish Caught " + count + " / " + boat.maxCaughtFish;
        }

        ShowMashKeys();

        Vector3 boatPos = boat.transform.position;

        if (mashKeyLeft != null)
        {
            mashKeyLeft.transform.position = boatPos + new Vector3(-2.5f, 0f, 0f);
            mashKeyLeft.transform.rotation = Quaternion.identity;
        }

        if (mashKeyRight != null)
        {
            mashKeyRight.transform.position = boatPos + new Vector3(2.5f, 0f, 0f);
            mashKeyRight.transform.rotation = Quaternion.identity;
        }
    }

    private void ShowMashKeys()
    {
        if (mashKeyLeft != null)
            mashKeyLeft.gameObject.SetActive(true);

        if (mashKeyRight != null)
            mashKeyRight.gameObject.SetActive(true);
    }

    private void HideMashUI()
    {
        if (caughtText != null)
            caughtText.gameObject.SetActive(false);

        if (mashKeyLeft != null)
            mashKeyLeft.gameObject.SetActive(false);

        if (mashKeyRight != null)
            mashKeyRight.gameObject.SetActive(false);
    }

    // ---------------- SCORE SYSTEM ----------------

    public void OnTrashCollected()
    {
        if (trashCountThisLevel <= 0) return;

        float pointsPerTrash = 10f / trashCountThisLevel; // Changed
        levelScore += pointsPerTrash;

        UpdateScoreUI();
    }

    public void OnFishCaught()
    {
        pendingFishPenalty += 2f; // Changed
        UpdateScoreUI();
    }

    public void OnFishReleased()
    {
        pendingFishPenalty -= 2f;
        if (pendingFishPenalty < 0f)
            pendingFishPenalty = 0f;

        UpdateScoreUI();
    }

    private void ApplyFishPenalty()
    {
        levelScore -= pendingFishPenalty;
        if (levelScore < 0f) levelScore = 0f;
        pendingFishPenalty = 0f;
    }

    private void UpdateScoreUI()
    {
        if (scoreText == null) return;

        scoreText.gameObject.SetActive(currentState == GameState.Play);

        scoreText.text =
            "Score: " + (totalScore + levelScore).ToString("F2") +
            "  Pending Penalty: - " + pendingFishPenalty.ToString("F2");
    }
}



