using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// PvP Summary scene controller.
/// Reads static data from PvPGameManager (Winner, P1Score, P2Score).
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
        Time.timeScale = 1f;

        if (winnerText  != null) winnerText.text  = PvPGameManager.Winner;
        if (p1ScoreText != null) p1ScoreText.text = $"P1 Score: {PvPGameManager.P1Score}";
        if (p2ScoreText != null) p2ScoreText.text = $"P2 Score: {PvPGameManager.P2Score}";

        if (rematchButton  != null) rematchButton.onClick.AddListener(()  => SceneManager.LoadScene("Level1_PvP"));
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }
}
