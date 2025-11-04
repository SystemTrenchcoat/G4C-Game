using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float forceAmount;
    public float spawnWaitTime;

    private float spawnTimer;
    private float perlinTimer;

    public enum RandomMethod { Pseudo, Gaussian, Perlin }
    public RandomMethod selectedMethod;

    //declared convenient testing
    public float min = -1f, max = 1f;
    public float gaussianMean = 0f, gaussianStandardDeviation = 1f;
    public float perlinStepAmount = 0.1f, perlinMin = -1f, perlinMax = 1f;

    //declared for broken/fix states
    public float chanceToBreak = 0.1f;
    public float chanceToRepair = 0.05f;
    private bool isBroken = false;

    //reference to spawner sprite render 
    private SpriteRenderer spawnerRenderer;

    private void Start()
    {
        spawnTimer = spawnWaitTime;
        spawnerRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //if borken, check for a chance to repair
        if (isBroken)
        {
            if (SuccessCheck(chanceToRepair))
            {
                isBroken = false;
                Debug.Log(gameObject.name + " repaired!");
            }
        }
        else
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                Spawn();
                spawnTimer = spawnWaitTime;

                //after spawn call, check for a chance to break
                if (SuccessCheck(chanceToBreak))
                {
                    isBroken = true;
                    Debug.Log(gameObject.name + " broke!");
                }
            }
        }
    }

    private float Pseudo(float min, float max)
    {
        return Random.Range(min, max);
    }

    private float Gaussian(float mean, float standardDeviation)
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + standardDeviation * randStdNormal;
    }

    private float Perlin(float stepAmount, float min, float max)
    {
        perlinTimer += stepAmount;
        float noise = Mathf.PerlinNoise(perlinTimer, 0);
        return Mathf.Lerp(min, max, noise);
    }

    private Vector2 CalcForce()
    {
        Vector2 force = transform.up * forceAmount;
        float randomRotation = 0;

        //case swap using enum
        switch (selectedMethod)
        {
            case RandomMethod.Pseudo:
                randomRotation = Pseudo(min, max);
                break;
            case RandomMethod.Gaussian:
                randomRotation = Gaussian(gaussianMean, gaussianStandardDeviation);
                break;
            case RandomMethod.Perlin:
                randomRotation = Perlin(perlinStepAmount, perlinMin, perlinMax);
                break;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, randomRotation);
        return rotation * force;
    }

    private void Spawn()
    {
        GameObject newObject = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();

        //apply spawner's sprite color to the object it spawns
        SpriteRenderer prefabRenderer = newObject.GetComponent<SpriteRenderer>();
        if (prefabRenderer != null && spawnerRenderer != null)
        {
            prefabRenderer.color = spawnerRenderer.color;
        }

        //apply force on rigibody
        if (rb != null)
        {
            rb.AddForce(CalcForce(), ForceMode2D.Impulse);
        }
    }

    private bool SuccessCheck(float probability)
    {
        return Random.value < probability;
    }
}

