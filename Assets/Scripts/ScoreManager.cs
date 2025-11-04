using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;//singleton
    public int score = 0;
    public TextMeshProUGUI scoreText;//UI Text to display score

    private void Awake()
    {
        //there can only be one scoreManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public void MultiplyScore(int multiplier)
    {
        score *= multiplier;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (!GameStateManager.Instance.IsDuringGame()) return;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        else
        {
            Debug.LogWarning("Score Text not assigned in the Inspector");
        }
    }
}
