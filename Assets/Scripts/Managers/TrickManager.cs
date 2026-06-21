using UnityEngine;

/// <summary>
/// Tracks airtime and awards trick points.
/// Reads GroundChecker.IsGrounded every FixedUpdate.
/// </summary>
public class TrickManager : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────
    [Header("References")]
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private ScoreManager  scoreManager;

    [Header("Toast UI (optional)")]
    [Tooltip("Call ShowToast(string) on this if assigned.")]
    [SerializeField] private HUDManager hudManager;   // may be null in PvP — set via HUDManager_PvP

    // ── State ──────────────────────────────────────────────────
    private bool  _wasGrounded = true;
    private float _airTimer;

    // ── Unity Lifecycle ────────────────────────────────────────
    private void Awake()
    {
        if (groundChecker == null)
            groundChecker = GetComponentInChildren<GroundChecker>();
        if (scoreManager == null)
            scoreManager = GetComponent<ScoreManager>();
    }

    private void FixedUpdate()
    {
        bool grounded = groundChecker != null && groundChecker.IsGrounded;

        if (!grounded)
        {
            _airTimer += Time.fixedDeltaTime;
        }
        else if (!_wasGrounded && grounded)
        {
            // Just landed
            EvaluateTrick(_airTimer);
            _airTimer = 0f;
        }

        _wasGrounded = grounded;
    }

    // ── Trick Evaluation ───────────────────────────────────────
    private void EvaluateTrick(float airTime)
    {
        if (airTime < 0.5f) return;

        if (airTime >= 1.0f)
        {
            scoreManager?.AddScore(300);
            scoreManager?.IncrementMultiplier();
            hudManager?.ShowToast("Big Air! +300");
        }
        else // 0.5 – 0.99 s
        {
            scoreManager?.AddScore(100);
            scoreManager?.IncrementMultiplier();
            hudManager?.ShowToast("Small Jump! +100");
        }
    }

    /// <summary>Called by CrashHandler on crash — resets the multiplier.</summary>
    public void OnCrash()
    {
        scoreManager?.ResetMultiplier();
        _airTimer = 0f;
    }
}
