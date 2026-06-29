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

        var pc = other.GetComponentInParent<PlayerController>() ?? other.GetComponent<PlayerController>();
        switch (type)
        {
            case PowerUpType.SpeedBoost:
                if (pc != null) pc.ApplySpeedBoost(duration);
                break;
            case PowerUpType.Invincibility:
                if (pc != null) pc.ApplyInvincibility(duration);
                break;
        }

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    // Speed boost and invincibility are now delegated to PlayerController.ApplySpeedBoost/ApplyInvincibility
}
