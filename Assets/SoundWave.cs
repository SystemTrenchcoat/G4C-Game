using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public float expandSpeed = 5f;     // how fast circle grows
    public float maxRadius = 8f;       // max size before it disappears
    public float duration = 2f;        // how long it exists
    private float timer = 0f;

    private CircleCollider2D col;
    private Vector3 startScale;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        startScale = transform.localScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // expand the circle visually
        float scale = Mathf.Lerp(startScale.x, maxRadius, timer / duration);
        transform.localScale = new Vector3(scale, scale, 1f);

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fish fish = collision.GetComponent<Fish>();
        if (fish != null)
        {
            // tell fish to react to the sound wave
            fish.OnSoundWaveHit(transform.position);
        }
    }
}

