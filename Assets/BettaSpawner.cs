using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BettaSpawner : MonoBehaviour
{
    //need bettafish prefab
    //have spawn interval
    [SerializeField] private GameObject bettaPrefab;
    public float spawnInterval = 5f;
    private float timer = 0f;

    void Update()
    {
        //timer goes up and spawn a bettafish
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBetta();
        }
    }

    //create bettafish near edges randomly
    private void SpawnBetta()
    {
        if (bettaPrefab == null)
        {
            Debug.LogError("Betta prefab is not assigned or destroyed!");
            return;
        }
        Vector3 spawnPos = GetRandomEdgePosition();
        Instantiate(bettaPrefab, spawnPos, Quaternion.identity);
    }

    private Vector3 GetRandomEdgePosition()
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float edgeOffset = 1.0f;//distance outside or inside the edge to spawn

        Vector3 spawnPosition = Vector3.zero;
        int edge = Random.Range(0, 4);

        if (edge == 0)//top
        {
            float x = Random.Range(-camWidth / 2f, camWidth / 2f);
            float y = (camHeight / 2f) + edgeOffset;
            spawnPosition = new Vector3(x, y, 0f);
        }
        else if (edge == 1)//bottom
        {
            float x = Random.Range(-camWidth / 2f, camWidth / 2f);
            float y = -(camHeight / 2f) - edgeOffset;
            spawnPosition = new Vector3(x, y, 0f);
        }
        else if (edge == 2)//left
        {
            float x = -(camWidth / 2f) - edgeOffset;
            float y = Random.Range(-camHeight / 2f, camHeight / 2f);
            spawnPosition = new Vector3(x, y, 0f);
        }
        else if (edge == 3)//right
        {
            float x = (camWidth / 2f) + edgeOffset;
            float y = Random.Range(-camHeight / 2f, camHeight / 2f);
            spawnPosition = new Vector3(x, y, 0f);
        }

        spawnPosition += cam.transform.position;//centered on Camera
        spawnPosition.z = 0f;

        return spawnPosition;
    }

}
