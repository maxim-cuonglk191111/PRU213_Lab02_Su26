using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Solo Score Summary scene controller.
/// Reads score from PlayerPrefs "LastScore"; updates HighScore if beaten.
/// Self-heals: auto-finds buttons/text by name if Inspector references are null.
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
        EnsureEventSystem();
        Time.timeScale = 1f;

        // Auto-find if Inspector refs missing
        if (currentScoreText == null) currentScoreText = FindTMP("Your_Score:_0");
        if (bestScoreText    == null) bestScoreText    = FindTMP("Best_Score:_0");
        if (replayButton     == null) replayButton     = FindBtn("Replay_Btn");
        if (mainMenuButton   == null) mainMenuButton   = FindBtn("Main_Menu_Btn");

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

    static Button FindBtn(string name) => GameObject.Find(name)?.GetComponent<Button>();
    static TextMeshProUGUI FindTMP(string name) => GameObject.Find(name)?.GetComponent<TextMeshProUGUI>();

    static void EnsureEventSystem()
    {
        if (FindObjectsByType<EventSystem>(FindObjectsInactive.Include).Length == 0)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }
}
