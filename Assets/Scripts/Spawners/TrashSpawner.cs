using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public List<GameObject> trashPrefabs; // assign in inspector
    public float minDistance = 1.0f; // how far apart trash must be

    private List<GameObject> spawnedTrash = new List<GameObject>();

    private GameObject ChooseTrash()
    {
        return trashPrefabs[Random.Range(0, trashPrefabs.Count)];
    }

    // Spawn trash for ONE level
    public void SpawnTrash(int count)
    {
        ClearAllTrash(); // ensure no leftovers

        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        for (int i = 0; i < count; i++)
        {
            //Debug.Log("Trash!");

            Vector2 pos = GetValidTrashSpawnPosition(camWidth, camHeight, cam);
            GameObject obj = Instantiate(ChooseTrash(), pos, Quaternion.identity);
            obj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

            //Debug.Log(obj);

            spawnedTrash.Add(obj);
        }
    }

    // Pick a random position inside camera bounds, ensuring no overlap
    private Vector2 GetValidTrashSpawnPosition(float camWidth, float camHeight, Camera cam)
    {
        const int maxAttempts = 20;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(-camWidth / 2f, camWidth / 2f),
                Random.Range(-camHeight / 2f, camHeight / 2f)
            );

            candidate += (Vector2)cam.transform.position;

            bool overlapped = false;

            foreach (GameObject t in spawnedTrash)
            {
                if (Vector2.Distance(t.transform.position, candidate) < minDistance)
                {
                    overlapped = true;
                    break;
                }
            }

            if (!overlapped)
                return candidate;
        }

        // fallback if crowded
        return cam.transform.position;
    }

    public void ClearAllTrash()
    {
        foreach (GameObject t in spawnedTrash)
        {
            if (t != null)
                Destroy(t);
        }

        spawnedTrash.Clear();
    }
}
