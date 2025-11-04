using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ChangeColor();//set the color at before game starts
    }

    private void OnEnable()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (GameStateManager.Instance == null) return;

        switch (GameStateManager.Instance.currentState)
        {
            case GameState.BeforeGameStarts:
                spriteRenderer.color = Color.red; //Red for BeforeGameStarts
                break;
            case GameState.DuringGame:
                spriteRenderer.color = Color.green; //Green for DuringGame
                break;
            case GameState.GameEnds:
                spriteRenderer.color = Color.blue; //Blue for GameEnds
                break;
        }
    }
}

