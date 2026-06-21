using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Solo Score Summary scene controller.
/// Reads score from PlayerPrefs "LastScore"; updates HighScore if beaten.
/// </summary>
public class ScoreSummaryManager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    [Header("Buttons")]
    [SerializeField] private Button replayButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        Time.timeScale = 1f;

        int current = PlayerPrefs.GetInt("LastScore", 0);
        int best    = PlayerPrefs.GetInt("HighScore",  0);

        if (current > best)
        {
            best = current;
            PlayerPrefs.SetInt("HighScore", best);
            PlayerPrefs.Save();
        }

        if (currentScoreText != null) currentScoreText.text = $"Your Score: {current}";
        if (bestScoreText    != null) bestScoreText.text    = $"Best Score: {best}";

        if (replayButton   != null) replayButton.onClick.AddListener(()   => SceneManager.LoadScene("Level1"));
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }
}
