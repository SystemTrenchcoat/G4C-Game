using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public GameObject spawnObject;
    public RectTransform spawnAreaUI;

    private Rect spawnArea;

    void Start()
    {
        DefineSpawnArea();
        UpdateSpawnAreaUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))//mouse left click
        {
            Vector2 mousePos = Input.mousePosition;

            if (spawnArea.Contains(mousePos))//is click inside the zone?
            {
                Spawn(mousePos);
            }
        }
    }

    /// <summary>
    /// spawn area at top of screen
    /// same width and 15% of the screen's height
    /// </summary>
    void DefineSpawnArea()
    {
        float width = Screen.width;        
        float height = Screen.height * 0.15f;

        float startX = 0;
        float startY = Screen.height - height;

        spawnArea = new Rect(startX, startY, width, height);
    }

    void UpdateSpawnAreaUI()
    {
        if (spawnAreaUI != null)
        {
            spawnAreaUI.sizeDelta = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            spawnAreaUI.anchoredPosition = Vector2.zero;
        }
    }

    void Spawn(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Instantiate(spawnObject, worldPos, Quaternion.identity);
    }
}
