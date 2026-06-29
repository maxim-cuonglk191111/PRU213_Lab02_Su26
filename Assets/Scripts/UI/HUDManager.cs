using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Solo-mode HUD: updates score, speed, lives icons, multiplier.
/// Also exposes ShowToast(string) used by TrickManager.
/// </summary>
public class HUDManager : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────
    [Header("References")]
    [SerializeField] private ScoreManager  scoreManager;
    [SerializeField] private LivesManager  livesManager;
    [SerializeField] private Rigidbody2D   playerRb;

    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI toastText;

    [Header("Lives Icons")]
    [SerializeField] private Image[] heartIcons;  // pre-placed in Canvas

    [Header("Toast")]
    [SerializeField] private float toastDuration = 1.5f;  // hold time at full opacity
    [SerializeField] private float fadeDuration   = 0.2f;

    // ── State ──────────────────────────────────────────────────
    private Coroutine _toastCoroutine;

    // ── Lifecycle ──────────────────────────────────────────────
    private void Awake()
    {
        if (toastText != null) toastText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (scoreManager != null) scoreManager.OnScoreChanged += UpdateScore;
        if (livesManager  != null) livesManager.OnLivesChanged += UpdateLives;
    }

    private void OnDisable()
    {
        if (scoreManager != null) scoreManager.OnScoreChanged -= UpdateScore;
        if (livesManager  != null) livesManager.OnLivesChanged -= UpdateLives;
    }

    private void Update()
    {
        UpdateSpeed();
        UpdateMultiplier();
    }

    // ── Update Methods ─────────────────────────────────────────
    private void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    private void UpdateSpeed()
    {
        if (speedText == null || playerRb == null) return;
        float kmh = playerRb.linearVelocity.magnitude * 3.6f;
        speedText.text = $"{kmh:F0} km/h";
    }

    private void UpdateMultiplier()
    {
        if (multiplierText == null || scoreManager == null) return;
        multiplierText.text = $"×{scoreManager.CurrentMultiplier}";
    }

    private void UpdateLives(int lives)
    {
        for (int i = 0; i < heartIcons.Length; i++)
            if (heartIcons[i] != null)
                heartIcons[i].enabled = i < lives;
    }

    // ── Public API ─────────────────────────────────────────────
    public void ShowToast(string message)
    {
        if (toastText == null) return;
        if (_toastCoroutine != null) StopCoroutine(_toastCoroutine);
        _toastCoroutine = StartCoroutine(ToastRoutine(message));
    }

    private IEnumerator ToastRoutine(string message)
    {
        toastText.text  = message;
        toastText.alpha = 0f;
        toastText.gameObject.SetActive(true);

        // Fade in
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            toastText.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        toastText.alpha = 1f;

        // Hold at full opacity
        yield return new WaitForSeconds(toastDuration);

        // Fade out
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            toastText.alpha = Mathf.Clamp01(1f - t / fadeDuration);
            yield return null;
        }

        toastText.gameObject.SetActive(false);
        _toastCoroutine = null;
    }
}
