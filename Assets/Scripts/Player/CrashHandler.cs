using System.Collections;
using UnityEngine;

/// <summary>
/// Attached to the Boarder_Top child collider.
/// Detects crash (head contact with Obstacle tag), deducts a life, plays SFX, and respawns.
/// </summary>
public class CrashHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The LivesManager belonging to this player's root GameObject.")]
    [SerializeField] private LivesManager livesManager;

    [Tooltip("The TrickManager belonging to this player (resets multiplier on crash).")]
    [SerializeField] private TrickManager trickManager;

    [Tooltip("AudioSource used to play crash SFX.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Crash sound effect clip.")]
    [SerializeField] private AudioClip crashClip;

    [Header("Respawn")]
    [Tooltip("Transform the boarder teleports to on respawn. If null, teleports to (0,0,0).")]
    [SerializeField] private Transform respawnPoint;

    [Tooltip("Duration (seconds) of post-crash invincibility.")]
    [SerializeField] private float invincibilityDuration = 1.5f;

    // ── State ──────────────────────────────────────────────────
    private bool _isInvincible;
    private Rigidbody2D _rb;
    private PlayerController _controller;

    private void Awake()
    {
        // Walk up to the player root to grab sibling components
        Transform root = transform.root;
        _rb         = root.GetComponent<Rigidbody2D>();
        _controller = root.GetComponent<PlayerController>();

        if (livesManager == null)
            livesManager = root.GetComponentInChildren<LivesManager>();
        if (trickManager == null)
            trickManager = root.GetComponentInChildren<TrickManager>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_isInvincible) return;
        if (!col.gameObject.CompareTag("Obstacle")) return;

        HandleCrash();
    }

    // ── Crash Logic ────────────────────────────────────────────
    private void HandleCrash()
    {
        // Play crash SFX
        if (audioSource != null && crashClip != null)
            audioSource.PlayOneShot(crashClip, 0.8f);

        // Reset trick multiplier
        trickManager?.OnCrash();

        // Deduct a life
        if (livesManager == null) return;
        livesManager.LoseLife();

        if (livesManager.CurrentLives > 0)
        {
            StartCoroutine(RespawnRoutine());
        }
        // If lives == 0, LivesManager itself fires the game-over / elimination event
    }

    private IEnumerator RespawnRoutine()
    {
        _isInvincible = true;
        _controller?.SetInputEnabled(false);

        // Stop physics
        if (_rb != null)
        {
            _rb.linearVelocity  = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

        // Teleport
        Transform root = transform.root;
        if (respawnPoint != null)
            root.position = respawnPoint.position;
        else
        {
            Debug.LogError("[CrashHandler] respawnPoint is null — teleporting to (0,0,0).");
            root.position = Vector3.zero;
        }
        root.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.1f);
        _controller?.SetInputEnabled(true);

        // Grace invincibility
        yield return new WaitForSeconds(invincibilityDuration - 0.1f);
        _isInvincible = false;
    }

    // ── Public API ─────────────────────────────────────────────
    /// <summary>External systems (PowerUp.cs) can grant temporary invincibility.</summary>
    public void SetInvincible(bool invincible) => _isInvincible = invincible;
}
