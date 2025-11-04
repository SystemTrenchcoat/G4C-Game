using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private bool gameOver = false;
    public TextMeshProUGUI restartText; // show restart message

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
        // Make sure time is normal at start
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }

        // Check if all Trash are cleaned
        if (!gameOver)
        {
            CheckTrashRemaining();
        }
    }

    // Checks how many Trash are still in the scene
    private void CheckTrashRemaining()
    {
        Trash[] allTrash = FindObjectsOfType<Trash>();

        if (allTrash.Length == 0)
        {
            EndGame();
        }
    }

    // End game when all trash are gone
    private void EndGame()
    {
        gameOver = true;
        Debug.Log("All trash cleaned! Press Space to Restart");

        if (restartText != null)
        {
            restartText.gameObject.SetActive(true);
            restartText.text = "All Trash Cleaned!\nPress Space to Restart";
        }

        Time.timeScale = 0f; // pause the game
    }

    // Restart scene when pressing Space
    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
