using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickupHandler : MonoBehaviour
{
    [SerializeField] private int      scoreValue = 50;
    [SerializeField] private AudioClip pickupClip;

    private bool _collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player") && !other.CompareTag("Player2")) return;

        _collected = true;

        var sm = other.GetComponentInParent<ScoreManager>();
        sm?.AddScore(scoreValue);

        if (AudioManager.Instance != null && pickupClip != null)
            AudioManager.Instance.PlaySFX(pickupClip, 0.7f);

        var ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.transform.SetParent(null);
            ps.Play();
            float particleTTL = Mathf.Max(ps.main.duration + ps.main.startLifetime.constant, 2f);
            Destroy(ps.gameObject, particleTTL);
        }

        Destroy(gameObject);
    }
}
