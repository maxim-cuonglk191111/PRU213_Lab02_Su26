using System.Collections;
using UnityEngine;

/// <summary>
/// Power-up trigger: Speed Boost or Invincibility.
/// Tag the zone as "PowerUp". Attach to the trigger collider.
/// </summary>
public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, Invincibility }

    [Header("Settings")]
    [SerializeField] private PowerUpType type           = PowerUpType.SpeedBoost;
    [SerializeField] private float       duration        = 5f;
    [SerializeField] private float       speedMultiplier = 1.5f;  // SpeedBoost only

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

        // Visually hide but keep script alive for coroutine
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    private IEnumerator ApplySpeedBoost(Rigidbody2D rb)
    {
        if (rb == null) yield break;
        rb.linearVelocity *= speedMultiplier;
        yield return new WaitForSeconds(duration);
        // Velocity naturally decays — no explicit reset needed
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
