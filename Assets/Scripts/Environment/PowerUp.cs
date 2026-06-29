using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, Invincibility }

    [Header("Settings")]
    [SerializeField] private PowerUpType type           = PowerUpType.SpeedBoost;
    [SerializeField] private float       duration        = 5f;
    [SerializeField] private float       speedMultiplier = 1.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Player2")) return;

        switch (type)
        {
            case PowerUpType.SpeedBoost:
                StartCoroutine(ApplySpeedBoost(other.GetComponentInParent<Rigidbody2D>()));
                break;
            case PowerUpType.Invincibility:
                StartCoroutine(ApplyInvincibility(other.GetComponentInParent<CrashHandler>()));
                break;
        }

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    private IEnumerator ApplySpeedBoost(Rigidbody2D rb)
    {
        if (rb == null) yield break;
        rb.linearVelocity *= speedMultiplier;

        var sprites    = rb.GetComponentsInChildren<SpriteRenderer>();
        var origColors = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            origColors[i] = sprites[i] ? sprites[i].color : Color.white;

        float elapsed = 0f;
        bool  tinted  = false;
        const float interval = 0.15f;
        while (elapsed < duration)
        {
            tinted = !tinted;
            for (int i = 0; i < sprites.Length; i++)
                if (sprites[i]) sprites[i].color = tinted ? Color.yellow : origColors[i];
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        for (int i = 0; i < sprites.Length; i++)
            if (sprites[i]) sprites[i].color = origColors[i];

        Destroy(gameObject);
    }

    private IEnumerator ApplyInvincibility(CrashHandler crashHandler)
    {
        if (crashHandler == null) yield break;
        crashHandler.SetInvincible(true);
        yield return new WaitForSeconds(duration);
        crashHandler.SetInvincible(false);
        Destroy(gameObject);
    }
}
