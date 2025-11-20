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
    public float levelTimeLimit = 105f;
    public int currentLevel = 1;
    public int maxLevels = 3;

    [Header("References")]
    public GameObject trashPrefab;
    public BettaSpawner spawner;
    public GameObject cooldownBarUI;

    // NEW --- SCORE SYSTEM ---
    [Header("NEW Score UI")]
    public TextMeshProUGUI scoreText; // Assign in Inspector
    private int totalScore = 0;        // Max 300
    private int levelScore = 0;        // Max 100 per level
    private int pendingFishPenalty = 0;
    private int trashCountThisLevel = 0;
    // NEW END ---

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
                "Ocean Cleanup\n\n" +
                "Move with mouse.\n\n" +
                "Right-click: release sound wave.\n\n" +
                "Avoid touching fish.\n\n" +
                "Press SPACE to start.";
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

        SpawnTrashForLevel(level);

        // recenter boat
        if (boat != null && Camera.main != null)
        {
            boat.transform.position = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                0f
            );
        }

        // --- RESET LEVEL SCORE DATA ---
        levelScore = 0;
        pendingFishPenalty = 0;
        trashCountThisLevel = GetTrashCountForLevel(level);
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

        currentState =
            (currentLevel < maxLevels) ? GameState.LevelEnd : GameState.End;

        // --- APPLY FISH PENALTY ---
        ApplyFishPenalty();
        totalScore += levelScore;
        UpdateScoreUI();

        if (infoText != null)
        {
            infoText.gameObject.SetActive(true);

            if (currentState == GameState.LevelEnd)
            {
                infoText.text =
                    "Level Complete\n" +
                    "Level Score: " + levelScore +
                    "\nTotal Score: " + totalScore +
                    "\nPress SPACE for next level";
            }
            else
            {
                float percent = (totalScore / 300f) * 100f;

                infoText.text =
                    "All Levels Complete\n" +
                    "Total Score: " + totalScore + " / 300\n" +
                    "Final Rating: " + percent.ToString("F0") + "%" +
                    "\nPress SPACE to restart";
            }
        }

        if (backgroundImage != null)
            backgroundImage.SetActive(true);
    }

    private void StartNextLevel()
    {
        int nextLevel = currentLevel + 1;

        if (nextLevel > maxLevels)
            EndGame();
        else
            StartLevel(nextLevel);
    }

    private void EndGame()
    {
        currentState = GameState.End;
        Time.timeScale = 0f;

        if (restartText != null)
        {
            float percent = (totalScore / 300f) * 100f;

            restartText.gameObject.SetActive(true);
            restartText.text =
                "All Levels Complete\n" +
                "Total Score: " + totalScore + " / 300\n" +
                "Final Rating: " + percent.ToString("F0") + "%" +
                "\nPress SPACE to restart";
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

    // ---------------- TRASH ----------------

    private void CheckTrashRemaining()
    {
        Trash[] allTrash = FindObjectsOfType<Trash>();
        if (allTrash.Length == 0)
            EndLevel();
    }

    private void SpawnTrashForLevel(int level)
    {
        if (trashPrefab == null)
        {
            Debug.LogError("Trash prefab not assigned!");
            return;
        }

        int count = GetTrashCountForLevel(level);

        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-camWidth / 2f, camWidth / 2f),
                Random.Range(-camHeight / 2f, camHeight / 2f),
                0f
            );

            pos.x += cam.transform.position.x;
            pos.y += cam.transform.position.y;

            GameObject trash = Instantiate(trashPrefab, pos, Quaternion.identity);
            trash.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
    }

    private int GetTrashCountForLevel(int level)
    {
        if (level == 1) return 3;
        if (level == 2) return 4;
        if (level == 3) return 6;
        return 3;
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

    // ============================================
    // --- SCORE SYSTEM
    // ============================================

    public void OnTrashCollected()
    {
        if (trashCountThisLevel <= 0) return;

        int pointsPerTrash = 100 / trashCountThisLevel;
        levelScore += pointsPerTrash;

        UpdateScoreUI();
    }

    public void OnFishCaught()
    {
        pendingFishPenalty += 20;
        UpdateScoreUI();
    }

    public void OnFishReleased()
    {
        pendingFishPenalty -= 20;
        if (pendingFishPenalty < 0) pendingFishPenalty = 0;

        UpdateScoreUI();
    }

    private void ApplyFishPenalty()
    {
        levelScore -= pendingFishPenalty;
        if (levelScore < 0) levelScore = 0;
        pendingFishPenalty = 0;
    }

    private void UpdateScoreUI()
    {
        if (scoreText == null) return;

        scoreText.gameObject.SetActive(currentState == GameState.Play);

        scoreText.text =
            "Score: " + (totalScore + levelScore) +
            "  Pending Penalty: - " + pendingFishPenalty;
    }
}

