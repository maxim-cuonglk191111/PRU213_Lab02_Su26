using System.Collections;
using UnityEngine;

public class CrashHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LivesManager livesManager;
    [SerializeField] private TrickManager trickManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip crashClip;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float invincibilityDuration = 1.5f;

    [Header("Upside-Down Detection")]
    [SerializeField] private float flipDeathAngle = 100f;

    [Header("Out-of-Bounds")]
    [SerializeField] private float killBelowY = -20f;

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

        if (livesManager == null) livesManager = root.GetComponentInChildren<LivesManager>();
        if (trickManager == null) trickManager = root.GetComponentInChildren<TrickManager>();
    }

    private void Update()
    {
        if (IsInvincible) return;

        Transform root = transform.root;

        if (root.position.y < killBelowY)
        {
            HandleCrash();
            return;
        }

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

    private void HandleCrash()
    {
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
            root.position = Vector3.zero;
        root.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.1f);
        _controller?.SetInputEnabled(true);

        yield return new WaitForSeconds(invincibilityDuration - 0.1f);
        _respawnInvincible = false;
        UpdateFlash();
    }

    public void SetInvincible(bool invincible)
    {
        _externalInvincible = invincible;
        UpdateFlash();
    }

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
