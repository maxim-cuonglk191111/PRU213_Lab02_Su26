using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScoreManager  scoreManager;
    [SerializeField] private LivesManager  livesManager;
    [SerializeField] private Rigidbody2D   playerRb;

    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI distText;

    [Header("Lives Icons")]
    [SerializeField] private Image[] heartIcons;

    [Header("Toast")]
    [SerializeField] private float toastDuration = 1.5f;
    [SerializeField] private float fadeDuration   = 0.2f;

    private Coroutine _toastCoroutine;
    private float _startXPos;

    private void Awake()
    {
        if (toastText != null) toastText.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (playerRb == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerRb = p.GetComponent<Rigidbody2D>();
        }
        _startXPos = playerRb != null ? playerRb.position.x : 0f;
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
        UpdateTimer();
        UpdateDistance();
    }

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

    private void UpdateTimer()
    {
        if (timerText == null) return;
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing) return;
        float t = Time.timeSinceLevelLoad;
        timerText.text = $"{Mathf.FloorToInt(t / 60f):00}:{Mathf.FloorToInt(t % 60f):00}";
    }

    private void UpdateDistance()
    {
        if (distText == null || playerRb == null) return;
        float dist = Mathf.Max(0f, playerRb.position.x - _startXPos);
        distText.text = $"{Mathf.RoundToInt(dist)}m";
    }

    private void UpdateLives(int lives)
    {
        for (int i = 0; i < heartIcons.Length; i++)
            if (heartIcons[i] != null)
                heartIcons[i].enabled = i < lives;
    }

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

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            toastText.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        toastText.alpha = 1f;

        yield return new WaitForSeconds(toastDuration);

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            toastText.alpha = Mathf.Clamp01(1f - t / fadeDuration);
            yield return null;
        }

        toastText.gameObject.SetActive(false);
        _toastCoroutine = null;
    }
}
