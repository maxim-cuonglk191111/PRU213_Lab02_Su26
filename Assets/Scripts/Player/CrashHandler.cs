using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashHandler : MonoBehaviour
{
    [Header("Crash Effect")]
    [SerializeField] ParticleSystem crashEffect;
    [SerializeField] float loadDelay = 1f;

    bool hasCrashed = false;
    PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Obstacle"))
        {
            HandleCrash();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.GetType() == typeof(CircleCollider2D))
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Obstacle"))
            {
                HandleCrash();
            }
        }
    }

    public void ForceCrash() => HandleCrash();

    void HandleCrash()
    {
        if (!hasCrashed && playerController != null && !playerController.isInvincible)
        {
            hasCrashed = true;
            playerController.DisableControls();

            if (crashEffect != null)
            {
                crashEffect.Play();
            }

            if (AudioManager.Instance != null) AudioManager.Instance.PlayCrashSound();

            var tm = GetComponent<TrickManager>();
            if (tm != null) tm.ResetCombo();

            Invoke(nameof(NotifyGameManager), loadDelay);
        }
    }

    void NotifyGameManager()
    {
        var lives = GetComponent<LivesManager>();
        if (lives == null) lives = GetComponentInParent<LivesManager>();

        if (lives != null)
        {
            lives.LoseLife();
            hasCrashed = false; // Reset crash state for next life
            playerController.ResetAll(); // Re-enable controls
            
            var respawn = GameObject.Find("RespawnPoint");
            if (respawn != null)
            {
                transform.position = respawn.transform.position;
                var rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    transform.rotation = Quaternion.identity;
                }
                
                var cam = Object.FindAnyObjectByType<CameraFollow>();
                if (cam != null) cam.SnapToPlayer();
            }
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerCrashed();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnEnable()
    {
        hasCrashed = false;
    }
}
