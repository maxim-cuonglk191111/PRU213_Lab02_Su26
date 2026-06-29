using UnityEngine;

public class TrickManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private HUDManager   hudManager;

    PlayerController playerController;
    bool  wasGrounded = true;
    float airTimer;

    void Awake()
    {
        if (scoreManager == null) scoreManager = GetComponent<ScoreManager>();
        playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        bool grounded = playerController == null || playerController.IsGrounded();

        if (!grounded)
        {
            airTimer += Time.fixedDeltaTime;
        }
        else if (!wasGrounded && grounded)
        {
            EvaluateTrick(airTimer);
            airTimer = 0f;
        }

        wasGrounded = grounded;
    }

    void EvaluateTrick(float airTime)
    {
        if (airTime < 0.5f) return;

        if (airTime >= 1.0f)
        {
            scoreManager?.AddScore(300);
            scoreManager?.IncrementMultiplier();
            hudManager?.ShowToast("Big Air! +300");
        }
        else
        {
            scoreManager?.AddScore(100);
            scoreManager?.IncrementMultiplier();
            hudManager?.ShowToast("Small Jump! +100");
        }
    }

    public void OnCrash()
    {
        scoreManager?.ResetMultiplier();
        airTimer = 0f;
    }
}
