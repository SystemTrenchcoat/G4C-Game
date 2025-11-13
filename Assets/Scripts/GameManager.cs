using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Start, Play, LevelEnd, End }
    public GameState currentState = GameState.Start;

    [Header("UI")]
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI restartText;
    public TextMeshProUGUI countdownText;
    public GameObject backgroundImage;

    [Header("Level Settings")]
    public float levelTimeLimit = 105f; // 1 min 45 sec
    public int currentLevel = 1;
    public int maxLevels = 3;

    [Header("References")]
    public GameObject trashPrefab;
    public BettaSpawner spawner;
    public GameObject cooldownBarUI; // assign your cooldown bar here

    private float levelTimer = 0f;
    private bool levelActive = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 0f;
        ShowStartMessage();

        if (spawner != null)
        {
            spawner.enabled = false;
        }

        // cooldown bar starts hidden
        if (cooldownBarUI != null)
        {
            cooldownBarUI.SetActive(false);
        }

        // level timer starts hidden
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
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
                "Right-click: release sound wave to disperse fish.\n\n" +
                "Clean all trash before time runs out!\n\n" +
                "Avoid touching the fish or you will have stop and release them!\n\n" +
                "Press SPACE to start.";
        }

        if (restartText != null) restartText.gameObject.SetActive(false);
        if (backgroundImage != null) backgroundImage.SetActive(true);
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

        // Enable cooldown bar
        if (cooldownBarUI != null)
        {
            cooldownBarUI.SetActive(true);
        }

        // Enable level timer
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        // Enable fish spawner
        if (spawner != null)
        {
            spawner.enabled = true;
            spawner.maxFishCount = 20; // fish cap
        }

        // Spawn trash
        SpawnTrashForLevel(currentLevel);

        // Recenter player at camera
        Boat playerBoat = FindObjectOfType<Boat>();
        if (playerBoat != null && Camera.main != null)
        {
            playerBoat.transform.position = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                0f
            );
        }

        Debug.Log($"Level {currentLevel} started!");
    }

    private void HandleLevelTimer()
    {
        if (!levelActive) return;

        levelTimer -= Time.deltaTime;

        // Update countdown text
        if (countdownText != null)
        {
            countdownText.text = $"{Mathf.CeilToInt(levelTimer)}s";
        }

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
        {
            spawner.enabled = false;
        }

        // Hide cooldown bar
        if (cooldownBarUI != null)
        {
            cooldownBarUI.SetActive(false);
        }

        // Hide level timer
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        currentState = (currentLevel < maxLevels) ? GameState.LevelEnd : GameState.End;

        if (infoText != null)
        {
            infoText.gameObject.SetActive(true);
            if (currentState == GameState.LevelEnd)
                infoText.text = $"Level {currentLevel} Complete!\nPress SPACE for next level.";
            else
                infoText.text = "All Levels Complete!\nPress SPACE to Restart.";
        }

        if (backgroundImage != null) backgroundImage.SetActive(true);
    }

    private void StartNextLevel()
    {
        int nextLevel = currentLevel + 1;
        if (nextLevel > maxLevels)
        {
            EndGame();
        }
        else
        {
            StartLevel(nextLevel);
        }
    }

    private void EndGame()
    {
        currentState = GameState.End;
        Time.timeScale = 0f;

        if (restartText != null)
        {
            restartText.gameObject.SetActive(true);
            restartText.text = "All Levels Complete!\nPress SPACE to Restart";
        }

        if (backgroundImage != null) backgroundImage.SetActive(true);

        // Hide cooldown bar
        if (cooldownBarUI != null)
            cooldownBarUI.SetActive(false);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

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
            Debug.LogError("Trash prefab not assigned in GameManager!");
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
        switch (level)
        {
            case 1: return 3;
            case 2: return 4;
            case 3: return 5;
            default: return 3;
        }
    }
}


