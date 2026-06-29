using System.Collections;
using UnityEngine;

/// <summary>
/// Attached to the Boarder_Top child collider.
/// Detects crashes via three paths:
///   1. Head hits an Obstacle-tagged object
///   2. Player rotation exceeds flipDeathAngle from upright (upside-down)
///   3. Player falls below killBelowY (out-of-bounds)
/// Sprites flash during invincibility from both crash respawn and power-up sources.
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

    [Header("Upside-Down Detection")]
    [Tooltip("Degrees from upright at which flipping counts as a crash. 90=sideways, 100=just-past-sideways, 120=generous.")]
    [SerializeField] private float flipDeathAngle = 100f;

    [Header("Out-of-Bounds")]
    [Tooltip("If the player's Y drops below this, they respawn (fell off the level).")]
    [SerializeField] private float killBelowY = -20f;

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

    private void Update()
    {
        if (IsInvincible) return;

        Transform root = transform.root;

        // Out-of-bounds: fell off the level
        if (root.position.y < killBelowY)
        {
            HandleCrash();
            return;
        }

        // Upside-down: eulerAngles.z in [0,360). Dead zone: (flipDeathAngle, 360-flipDeathAngle)
        float angle = root.eulerAngles.z;
        if (angle > flipDeathAngle && angle < 360f - flipDeathAngle)
            HandleCrash();
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
        // Set invincibility immediately to block re-entrant triggers this frame
        _respawnInvincible = true;
        UpdateFlash();

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
        // _respawnInvincible is already true, set by HandleCrash
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
