using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Start, Play, LevelEnd, End } // NEW: added LevelEnd
    public GameState currentState = GameState.Start;

    [Header("UI")]
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI restartText;
    public GameObject backgroundImage;

    [Header("Level Settings")]
    public float levelTimeLimit = 105f;   // 1 min 45 sec
    public int currentLevel = 1;
    public int maxLevels = 3;
    public BettaSpawner spawner;

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
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Start:
                if (Input.GetKeyDown(KeyCode.Space))
                    StartLevel(1); // begin level 1
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
                "Ocean Cleanup \n\n" +
                "Move with mouse.\nRight-click: sound wave.\n" +
                "Clean all trash before time runs out!\n\n" +
                "Press SPACE to start.";
        }

        if (restartText != null) restartText.gameObject.SetActive(false);
        if (backgroundImage != null) backgroundImage.SetActive(true);
    }

    // ---------------------- LEVEL CONTROL ----------------------

    private void StartLevel(int level)
    {
        currentLevel = level;
        currentState = GameState.Play;
        Time.timeScale = 1f;

        if (infoText != null) infoText.gameObject.SetActive(false);
        if (backgroundImage != null) backgroundImage.SetActive(false);

        levelTimer = levelTimeLimit;
        levelActive = true;

        BettaSpawner spawner = FindObjectOfType<BettaSpawner>();
        if (spawner != null)
        {
            spawner.maxFishCount = GetTrashCountForLevel(currentLevel);
        }
    }

    private void HandleLevelTimer()
    {
        if (!levelActive) return;

        levelTimer -= Time.deltaTime;

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
        currentState = (currentLevel < maxLevels) ? GameState.LevelEnd : GameState.End;

        if (infoText != null)
        {
            infoText.gameObject.SetActive(true);
            if (currentState == GameState.LevelEnd)
                infoText.text = $"Level {currentLevel} Complete!\nPress SPACE for next level.";
            else
                infoText.text = "All Trash Cleaned!\nPress SPACE to Restart.";
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

    private int GetTrashCountForLevel(int level)
    {
        switch (level)
        {
            case 1: return 3;
            case 2: return 4;
            case 3: return 6;
            default: return 3;
        }
    }
}
