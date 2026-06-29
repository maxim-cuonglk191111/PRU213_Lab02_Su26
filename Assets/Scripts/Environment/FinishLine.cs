using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("Finish Settings")]
    public int finishBonus = 1000;
    public ParticleSystem finishParticles;
    public AudioClip finishSound;
    public Color triggerColor = new Color(1f, 1f, 1f, 0.3f);

    [Header("Mode")]
    public bool isPvP = false;

    bool hasFinished = false;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && triggerColor.a > 0f)
        {
            spriteRenderer.color = triggerColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool isP1 = other.CompareTag("Player");
        bool isP2 = other.CompareTag("Player2");

        if (!isP1 && !isP2) return;
        if (hasFinished) return;

        hasFinished = true;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) pc = other.GetComponentInParent<PlayerController>();
        if (pc != null) pc.SetFinished();

        if (finishParticles != null)
            finishParticles.Play();

        if (finishSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(finishSound);
            }
            else if (AudioManager.Instance != null)
            {
                AudioManager.Instance.sfxSource.PlayOneShot(finishSound);
            }
        }
        else if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFinishSound();
        }

        if (isPvP)
        {
            PvPGameManager.Instance?.OnFinishLineCrossed(isP1 ? "Player" : "Player2");
        }
        else
        {
            var sm = other.GetComponent<ScoreManager>();
            if (sm != null) sm.AddScore(finishBonus);

            Invoke(nameof(TriggerFinish), 1f);
        }
    }

    void TriggerFinish()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LevelComplete(false);
    }

    void OnEnable()
    {
        hasFinished = false;
    }
}
