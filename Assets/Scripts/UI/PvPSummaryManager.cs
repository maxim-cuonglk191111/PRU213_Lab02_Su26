using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// PvP Summary scene controller.
/// Reads static data from PvPGameManager (Winner, P1Score, P2Score).
/// Self-heals: auto-finds buttons/text by name if Inspector references are null.
/// </summary>
public class PvPSummaryManager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI p1ScoreText;
    [SerializeField] private TextMeshProUGUI p2ScoreText;

    [Header("Buttons")]
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        EnsureEventSystem();
        Time.timeScale = 1f;

        // Auto-find if Inspector refs missing
        if (winnerText    == null) winnerText    = FindTMP("Player_1_Wins!");
        if (p1ScoreText   == null) p1ScoreText   = FindTMP("P1_Score:_0");
        if (p2ScoreText   == null) p2ScoreText   = FindTMP("P2_Score:_0");
        if (rematchButton  == null) rematchButton  = FindBtn("Rematch_Btn");
        if (mainMenuButton == null) mainMenuButton = FindBtn("Main_Menu_Btn");

        if (winnerText  != null) winnerText.text  = PvPGameManager.Winner;
        if (p1ScoreText != null) p1ScoreText.text = $"P1 Score: {PvPGameManager.P1Score}";
        if (p2ScoreText != null) p2ScoreText.text = $"P2 Score: {PvPGameManager.P2Score}";

        if (rematchButton  != null) rematchButton.onClick.AddListener(()  => SceneManager.LoadScene("Level1_PvP"));
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    static Button FindBtn(string name) => GameObject.Find(name)?.GetComponent<Button>();
    static TextMeshProUGUI FindTMP(string name) => GameObject.Find(name)?.GetComponent<TextMeshProUGUI>();

    static void EnsureEventSystem()
    {
        if (FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length == 0)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }
}
