using System.Collections;
using UnityEngine;

public class CrashHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LivesManager livesManager;
    [SerializeField] private TrickManager trickManager;
    [SerializeField] private AudioSource  audioSource;
    [SerializeField] private AudioClip    crashClip;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float     respawnDelay = 1f;

    [Header("Out-of-Bounds")]
    [SerializeField] private float killBelowY = -20f;

    bool             hasCrashed;
    bool             externalInvincible;
    Coroutine        flashCoroutine;
    PlayerController playerController;
    SpriteRenderer[] sprites;

    void Awake()
    {
        Transform root = transform.root;
        playerController = root.GetComponent<PlayerController>();
        sprites          = root.GetComponentsInChildren<SpriteRenderer>();

        if (livesManager == null) livesManager = root.GetComponentInChildren<LivesManager>();
        if (trickManager == null) trickManager = root.GetComponentInChildren<TrickManager>();
    }

    void Update()
    {
        if (hasCrashed || externalInvincible) return;
        if (transform.root.position.y < killBelowY)
            HandleCrash();
    }

    // Head trigger collider hits terrain tagged "Ground" → crash
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Obstacle"))
            HandleCrash();
    }

    // Board (CircleCollider2D on root) hits terrain
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.GetType() == typeof(CircleCollider2D) &&
            (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Obstacle")))
            HandleCrash();
    }

    public void SetInvincible(bool value)
    {
        externalInvincible = value;
        if (value)
        {
            if (flashCoroutine == null) flashCoroutine = StartCoroutine(ExternalInvincibilityFlash());
        }
        else
        {
            if (flashCoroutine != null) { StopCoroutine(flashCoroutine); flashCoroutine = null; }
            foreach (var sr in sprites) if (sr != null) sr.enabled = true;
        }
    }

    IEnumerator ExternalInvincibilityFlash()
    {
        bool visible = true;
        while (externalInvincible)
        {
            visible = !visible;
            foreach (var sr in sprites) if (sr != null) sr.enabled = visible;
            yield return new WaitForSeconds(0.15f);
        }
        foreach (var sr in sprites) if (sr != null) sr.enabled = true;
        flashCoroutine = null;
    }

    void HandleCrash()
    {
        if (hasCrashed || externalInvincible) return;
        hasCrashed = true;

        playerController?.DisableControls();

        if (audioSource != null && crashClip != null)
            audioSource.PlayOneShot(crashClip, 0.8f);
        else
            AudioManager.Instance?.PlayCrashSound();

        trickManager?.OnCrash();

        if (livesManager != null)
        {
            livesManager.LoseLife();
            if (livesManager.CurrentLives > 0)
                StartCoroutine(RespawnRoutine());
            else
                Invoke(nameof(NotifyGameOver), respawnDelay);
        }
        else
        {
            Invoke(nameof(NotifyGameOver), respawnDelay);
        }
    }

    IEnumerator RespawnRoutine()
    {
        Transform root = transform.root;
        var rb = root.GetComponent<Rigidbody2D>();
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.angularVelocity = 0f; }

        yield return new WaitForSeconds(respawnDelay);

        if (respawnPoint != null)
            root.SetPositionAndRotation(respawnPoint.position, Quaternion.identity);

        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.angularVelocity = 0f; }

        playerController?.EnableControls();
        StartCoroutine(InvincibilityFlash(1.5f));
        hasCrashed = false;
    }

    IEnumerator InvincibilityFlash(float duration)
    {
        float elapsed = 0f;
        bool  visible = true;
        while (elapsed < duration)
        {
            visible = !visible;
            foreach (var sr in sprites) if (sr != null) sr.enabled = visible;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        foreach (var sr in sprites) if (sr != null) sr.enabled = true;
    }

    void NotifyGameOver()
    {
        // PvP: LivesManager.OnPlayerEliminated event is already wired to PvPGameManager
        if (PvPGameManager.Instance != null) return;
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerCrashed();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void OnEnable() { hasCrashed = false; }
}
