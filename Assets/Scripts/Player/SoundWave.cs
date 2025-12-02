using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public float expandSpeed = 5f;
    public float maxRadius = 8f;
    public float duration = 2f;
    private float timer = 0f;

    private CircleCollider2D col;
    private Vector3 startScale;
    private float currentRadius = 0f;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        startScale = transform.localScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Expand radius
        currentRadius = Mathf.Lerp(0f, maxRadius, timer / duration);
        transform.localScale = new Vector3(currentRadius, currentRadius, 1f);

        // Notify all fish inside the wave
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRadius);
        foreach (var hit in hits)
        {
            Fish fish = hit.GetComponent<Fish>();
            if (fish != null)
            {
                fish.OnSoundWaveHit(transform.position);
            }
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}


