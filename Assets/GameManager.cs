using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState // new
    {
        Start,
        Play,
        End
    }

    public GameState currentState = GameState.Start; // new

    public TextMeshProUGUI infoText;   // new: show instructions or goal
    public TextMeshProUGUI restartText;
    public GameObject backgroundImage;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 0f; // new: pause at the start
        ShowStartMessage();  // new
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Start:
                if (Input.GetKeyDown(KeyCode.Space))
                    StartGame(); // new
                break;

            case GameState.Play:
                CheckTrashRemaining(); // same as before
                break;

            case GameState.End:
                if (Input.GetKeyDown(KeyCode.Space))
                    RestartGame(); // same as before
                break;
        }
    }

    // --- new ---
    private void ShowStartMessage()
    {
        if (infoText != null)
        {
            infoText.gameObject.SetActive(true);
            infoText.text =
                " Ocean Cleanup \n\n" +
                "Move your boat with the mouse.\n" +
                "Right-click to send sound waves and scare away the fish.\n" +
                "Clean all the trash!\n\n" +
                "Press SPACE to begin.";
        }

        if (restartText != null) 
        {
            restartText.gameObject.SetActive(false);
        }

        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(true);
        }
    }

    // --- new ---
    private void StartGame()
    {
        currentState = GameState.Play;
        Time.timeScale = 1f;

        if (infoText != null) 
        {
            infoText.gameObject.SetActive(false);
        }

        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(false);
        }
    }

    private void CheckTrashRemaining()
    {
        Trash[] allTrash = FindObjectsOfType<Trash>();
        if (allTrash.Length == 0) 
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        currentState = GameState.End;
        Debug.Log("All trash cleaned! Press Space to Restart");

        Time.timeScale = 0f;

        if (restartText != null)
        {
            restartText.gameObject.SetActive(true);
            restartText.text = "All Trash Cleaned!\nPress SPACE to Restart";
        }

        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(true);
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
