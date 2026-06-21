using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PvP HUD manager: manages two HUD panels (one per player).
/// Applies per-player border color: P1 = #00AAFF (blue), P2 = #FF6B35 (orange).
/// </summary>
public class HUDManager_PvP : MonoBehaviour
{
    // ── Per-Player Color Constants ─────────────────────────────
    private static readonly Color P1BorderColor = new Color(0f,    0.667f, 1f,    1f);  // #00AAFF
    private static readonly Color P2BorderColor = new Color(1f,    0.420f, 0.208f, 1f); // #FF6B35

    // ── Inspector ──────────────────────────────────────────────
    [Header("Player 1 HUD Panel")]
    [SerializeField] private Image           p1BorderPanel;
    [SerializeField] private ScoreManager    p1ScoreManager;
    [SerializeField] private LivesManager    p1LivesManager;
    [SerializeField] private Rigidbody2D     p1Rb;
    [SerializeField] private TextMeshProUGUI p1ScoreText;
    [SerializeField] private TextMeshProUGUI p1SpeedText;
    [SerializeField] private TextMeshProUGUI p1MultiplierText;
    [SerializeField] private Image[]         p1HeartIcons;

    [Header("Player 2 HUD Panel")]
    [SerializeField] private Image           p2BorderPanel;
    [SerializeField] private ScoreManager    p2ScoreManager;
    [SerializeField] private LivesManager    p2LivesManager;
    [SerializeField] private Rigidbody2D     p2Rb;
    [SerializeField] private TextMeshProUGUI p2ScoreText;
    [SerializeField] private TextMeshProUGUI p2SpeedText;
    [SerializeField] private TextMeshProUGUI p2MultiplierText;
    [SerializeField] private Image[]         p2HeartIcons;

    // ── Lifecycle ──────────────────────────────────────────────
    private void Awake()
    {
        // Apply border colors (PRD A9)
        if (p1BorderPanel != null) p1BorderPanel.color = P1BorderColor;
        if (p2BorderPanel != null) p2BorderPanel.color = P2BorderColor;
    }

    private void OnEnable()
    {
        if (p1ScoreManager != null) p1ScoreManager.OnScoreChanged += UpdateP1Score;
        if (p1LivesManager  != null) p1LivesManager.OnLivesChanged  += UpdateP1Lives;
        if (p2ScoreManager != null) p2ScoreManager.OnScoreChanged += UpdateP2Score;
        if (p2LivesManager  != null) p2LivesManager.OnLivesChanged  += UpdateP2Lives;
    }

    private void OnDisable()
    {
        if (p1ScoreManager != null) p1ScoreManager.OnScoreChanged -= UpdateP1Score;
        if (p1LivesManager  != null) p1LivesManager.OnLivesChanged  -= UpdateP1Lives;
        if (p2ScoreManager != null) p2ScoreManager.OnScoreChanged -= UpdateP2Score;
        if (p2LivesManager  != null) p2LivesManager.OnLivesChanged  -= UpdateP2Lives;
    }

    private void Update()
    {
        UpdateSpeed(p1Rb, p1SpeedText);
        UpdateSpeed(p2Rb, p2SpeedText);
        UpdateMultiplier(p1ScoreManager, p1MultiplierText);
        UpdateMultiplier(p2ScoreManager, p2MultiplierText);
    }

    // ── Score ──────────────────────────────────────────────────
    private void UpdateP1Score(int score) { if (p1ScoreText) p1ScoreText.text = $"Score: {score}"; }
    private void UpdateP2Score(int score) { if (p2ScoreText) p2ScoreText.text = $"Score: {score}"; }

    // ── Speed ──────────────────────────────────────────────────
    private static void UpdateSpeed(Rigidbody2D rb, TextMeshProUGUI label)
    {
        if (rb == null || label == null) return;
        label.text = $"{rb.linearVelocity.magnitude * 3.6f:F0} km/h";
    }

    // ── Multiplier ─────────────────────────────────────────────
    private static void UpdateMultiplier(ScoreManager sm, TextMeshProUGUI label)
    {
        if (sm == null || label == null) return;
        label.text = $"×{sm.CurrentMultiplier}";
    }

    // ── Lives ──────────────────────────────────────────────────
    private void UpdateP1Lives(int lives) => RefreshHearts(p1HeartIcons, lives);
    private void UpdateP2Lives(int lives) => RefreshHearts(p2HeartIcons, lives);

    private static void RefreshHearts(Image[] icons, int lives)
    {
        if (icons == null) return;
        for (int i = 0; i < icons.Length; i++)
            if (icons[i] != null) icons[i].enabled = i < lives;
    }
}
