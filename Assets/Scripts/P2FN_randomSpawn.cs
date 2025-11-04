using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2FN_randomSpawn : MonoBehaviour
{
    //arrat of fruits
    public GameObject[] fruitPrefabs;

    //range for launch force
    public float minForce = 5f; 
    public float maxForce = 10f;

    //range of angular torque on launched fruits
    public float minTorque = -50f; 
    public float maxTorque = 50f;

    //range for random spawn timing
    public float minSpawnWaitTime = 0.5f; 
    public float maxSpawnWaitTime = 2f;

    //area where fruits can spawn (bottom of the screen)
    public Vector2 spawnAreaMin = new Vector2(-5f, -4f); // Bottom-left corner
    public Vector2 spawnAreaMax = new Vector2(5f, -4f);  // Bottom-right corner

    //timer for spawn intervals
    private float spawnTimer;

    private void Start()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        if (!GameStateManager.Instance.IsDuringGame()) return;

        //decrease the spawn timer
        spawnTimer -= Time.deltaTime;

        //when the timer reaches zero, spawn a fruit and reset the timer
        if (spawnTimer <= 0)
        {
            Spawn();
            ResetSpawnTimer();
        }
    }

    private Vector2 CalcForce()
    {
        //random force here
        float forceMagnitude = Random.Range(minForce, maxForce);

        //random launch angle here
        float randomRotation = Random.Range(-30f, 30f);

        Quaternion rotation = Quaternion.Euler(0, 0, randomRotation);
        return rotation * (transform.up * forceMagnitude);
    }

    private void ApplyTorque(Rigidbody2D rb)
    {
        float torque = Random.Range(minTorque, maxTorque);
        rb.AddTorque(torque, ForceMode2D.Impulse);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        return new Vector2(randomX, spawnAreaMin.y);
    }

    private GameObject GetRandomFruitPrefab()
    {
        if (fruitPrefabs.Length == 0) return null;
        int randomIndex = Random.Range(0, fruitPrefabs.Length);
        return fruitPrefabs[randomIndex];
    }

    private void Spawn()
    {
        //get random fruit prefab
        GameObject fruitPrefab = GetRandomFruitPrefab();
        if (fruitPrefab == null) return;

        //get random spawn position
        Vector2 spawnPosition = GetRandomSpawnPosition();

        //instantiate the fruit at the spawn position
        GameObject newObject = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);

        //apply force and torque to the fruit
        Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(CalcForce(), ForceMode2D.Impulse);
            ApplyTorque(rb);
        }
    }

    //reset the spawn timer with a random value within a range
    private void ResetSpawnTimer()
    {
        spawnTimer = Random.Range(minSpawnWaitTime, maxSpawnWaitTime);
    }
}