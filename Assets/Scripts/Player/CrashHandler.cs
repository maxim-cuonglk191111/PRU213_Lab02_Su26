using System.Collections;
using UnityEngine;

/// <summary>
/// Attached to the Boarder_Top child collider.
/// Detects crash (head contact with Obstacle tag), deducts a life, plays SFX, and respawns.
/// Sprites flash during invincibility from both crash respawn and external power-up sources.
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
    private bool _respawnInvincible;
    private bool _externalInvincible;
    private bool IsInvincible => _respawnInvincible || _externalInvincible;

    private Rigidbody2D      _rb;
    private PlayerController _controller;
    private SpriteRenderer[] _sprites;
    private Coroutine        _flashCoroutine;

    private void Awake()
    {
        Transform root = transform.root;
        _rb         = root.GetComponent<Rigidbody2D>();
        _controller = root.GetComponent<PlayerController>();
        _sprites    = root.GetComponentsInChildren<SpriteRenderer>();

        if (livesManager == null)
            livesManager = root.GetComponentInChildren<LivesManager>();
        if (trickManager == null)
            trickManager = root.GetComponentInChildren<TrickManager>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (IsInvincible) return;
        if (!col.gameObject.CompareTag("Obstacle")) return;
        HandleCrash();
    }

    // ── Crash Logic ────────────────────────────────────────────
    private void HandleCrash()
    {
        if (audioSource != null && crashClip != null)
            audioSource.PlayOneShot(crashClip, 0.8f);

        trickManager?.OnCrash();

        if (livesManager == null) return;
        livesManager.LoseLife();

        if (livesManager.CurrentLives > 0)
            StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        _respawnInvincible = true;
        UpdateFlash();
        _controller?.SetInputEnabled(false);

        if (_rb != null)
        {
            _rb.linearVelocity  = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

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

        yield return new WaitForSeconds(invincibilityDuration - 0.1f);
        _respawnInvincible = false;
        UpdateFlash();
    }

    // ── Public API ─────────────────────────────────────────────
    /// <summary>External systems (PowerUp.cs) can grant temporary invincibility.</summary>
    public void SetInvincible(bool invincible)
    {
        _externalInvincible = invincible;
        UpdateFlash();
    }

    // ── Flash ──────────────────────────────────────────────────
    private void UpdateFlash()
    {
        if (IsInvincible)
        {
            if (_flashCoroutine == null)
                _flashCoroutine = StartCoroutine(FlashCoroutine());
        }
        else
        {
            if (_flashCoroutine != null) { StopCoroutine(_flashCoroutine); _flashCoroutine = null; }
            SetSpritesVisible(true);
        }
    }

    private IEnumerator FlashCoroutine()
    {
        bool visible = true;
        while (true)
        {
            visible = !visible;
            SetSpritesVisible(visible);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SetSpritesVisible(bool visible)
    {
        foreach (var sr in _sprites)
            if (sr != null) sr.enabled = visible;
    }
}
