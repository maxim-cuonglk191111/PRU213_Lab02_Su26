using UnityEngine;

/// <summary>
/// Snowflake collectible pickup handler.
/// Attach to each Snowflake prefab root (which has a CircleCollider2D with IsTrigger = true).
/// Uses GetComponentInParent to find the correct player's ScoreManager in PvP mode.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PickupHandler : MonoBehaviour
{
    [SerializeField] private int     scoreValue = 50;
    [SerializeField] private AudioClip pickupClip;

    private bool _collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player") && !other.CompareTag("Player2")) return;

        _collected = true;

        // Find the player's ScoreManager (works for both Solo and PvP)
        var sm = other.GetComponentInParent<ScoreManager>();
        sm?.AddScore(scoreValue);

        // Play pickup SFX
        if (AudioManager.Instance != null && pickupClip != null)
            AudioManager.Instance.PlaySFX(pickupClip, 0.7f);

        // Spawn particle burst (optional — handled by a ParticleSystem child, if present)
        var ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.transform.SetParent(null);
            ps.Play();
            // constantMax returns 0 for curve-based lifetimes; .constant is safer, with a 2s minimum
            float particleTTL = Mathf.Max(ps.main.duration + ps.main.startLifetime.constant, 2f);
            Destroy(ps.gameObject, particleTTL);
        }

        Destroy(gameObject);
    }
}
