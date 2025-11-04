using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState currentState;

    //test objects for visual game state swapping
    public GameObject beforeGameStateObject;
    public GameObject duringGameStateObject;
    public GameObject gameEndStateObject;

    public TextMeshProUGUI gameStateText; // UI text to display the current state
    public TextMeshProUGUI timerText; // UI text to display the timer
    public TextMeshProUGUI scoreText;

    private float gameTimer = 75f; // 75-second timer for the DuringGame state

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SwitchState(GameState.BeforeGameStarts);//now its game start state be default
    }

    private void Update()
    {
        // Update the timer and UI during the DuringGame state
        if (currentState == GameState.DuringGame)
        {
            gameTimer -= Time.deltaTime;
            UpdateTimerUI();

            // Switch to GameEnds state when the timer reaches zero
            if (gameTimer <= 0)
            {
                SwitchState(GameState.GameEnds);
            }
        }

        //space key for state change
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CycleState();
        }
    }

    private void CycleState()
    {
        //switch between different states
        switch (currentState)
        {
            case GameState.BeforeGameStarts:
                SwitchState(GameState.DuringGame);
                break;
            case GameState.DuringGame:
                SwitchState(GameState.GameEnds);
                break;
            case GameState.GameEnds:
                SwitchState(GameState.BeforeGameStarts);
                break;
        }
    }

    public void SwitchState(GameState newState)
    {
        currentState = newState;

        //only activiate the current state's game object
        beforeGameStateObject.SetActive(newState == GameState.BeforeGameStarts);
        duringGameStateObject.SetActive(newState == GameState.DuringGame);
        gameEndStateObject.SetActive(newState == GameState.GameEnds);

        // Update the UI text based on the current state
        UpdateStateUI();

        // Reset the timer when entering the DuringGame state
        if (newState == GameState.DuringGame)
        {
            gameTimer = 45f; // Reset the timer to 75 seconds
        }

        if (newState == GameState.GameEnds)
        {
            DisplayFinalScore();
        }
    }

    private void UpdateStateUI()
    {
        if (gameStateText != null)
        {
            switch (currentState)
            {
                case GameState.BeforeGameStarts:
                    gameStateText.text = "press space to start";
                    break;
                case GameState.DuringGame:
                    gameStateText.text = "Game on";
                    break;
                case GameState.GameEnds:
                    gameStateText.text = "Game Ends";
                    break;
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(gameTimer).ToString();
        }
    }

    public bool IsDuringGame()
    {
        return currentState == GameState.DuringGame;
    }

    private void DisplayFinalScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Game ends, your final score is: " + ScoreManager.Instance.score;
        }
        else
        {
            Debug.LogWarning("Score Text not assigned in the Inspector");
        }
    }
}
