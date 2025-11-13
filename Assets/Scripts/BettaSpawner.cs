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
    [SerializeField] public int maxFishCount = 20; // adjustable in Inspector
    private List<List<Fish>> packs = new List<List<Fish>>();
    private bool packsAssigned = false;

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

    private void SpawnBetta()
    {
        int currentFishCount = FindObjectsOfType<Fish>().Length;
        if (currentFishCount >= maxFishCount)
            return;

        Vector3 spawnPos = GetRandomEdgePosition();
        GameObject fishObj = Instantiate(bettaPrefab, spawnPos, Quaternion.identity);
        Fish newFish = fishObj.GetComponent<Fish>();

        // after reaching full fish count, assign packs once
        if (!packsAssigned)
        {
            StartCoroutine(AssignFishPacks());
        }
    }

    private IEnumerator AssignFishPacks()
    {
        yield return new WaitForSeconds(1f); // wait for all fish to spawn

        Fish[] allFish = FindObjectsOfType<Fish>();
        int totalFish = allFish.Length;
        int numPacks = Random.Range(2, 5); // between 2 and 4
        int fishPerPack = 4;
        int usedFish = 0;

        List<Fish> fishList = new List<Fish>(allFish);
        packs.Clear();

        for (int i = 0; i < numPacks; i++)
        {
            if (usedFish + fishPerPack > totalFish)
                break;

            List<Fish> pack = new List<Fish>();
            for (int j = 0; j < fishPerPack; j++)
            {
                pack.Add(fishList[usedFish]);
                usedFish++;
            }

            // first fish in each pack = leader
            pack[0].isLeader = true;

            // the rest follow the leader
            for (int k = 1; k < pack.Count; k++)
            {
                pack[k].leaderFish = pack[0];
                pack[k].isLeader = false;
            }

            packs.Add(pack);
        }

        packsAssigned = true;
        Debug.Log($"Assigned {packs.Count} fish packs");
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
