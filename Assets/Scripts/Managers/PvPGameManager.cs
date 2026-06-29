using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PvPGameManager : MonoBehaviour
{
    public static PvPGameManager Instance { get; private set; }

    [Header("Player References")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [Header("Per-Player Managers")]
    [SerializeField] private LivesManager livesManager1;
    [SerializeField] private LivesManager livesManager2;
    [SerializeField] private ScoreManager scoreManager1;
    [SerializeField] private ScoreManager scoreManager2;

    public static string Winner = "";
    public static int    P1Score = 0;
    public static int    P2Score = 0;

    private bool _gameEnded;
    private bool _paused;

    private void Awake()
    {
        Instance = this;
        Winner   = "";

        if (player1 == null || player2 == null)
        {
            Debug.LogError("[PvPGameManager] Player1 or Player2 not assigned.");
            SceneManager.LoadScene("Level1");
            return;
        }

        if (livesManager1 != null) livesManager1.OnPlayerEliminated += OnPlayerEliminated;
        if (livesManager2 != null) livesManager2.OnPlayerEliminated += OnPlayerEliminated;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_paused) PauseGame();
            else ResumeGame();
        }
    }

    public void OnFinishLineCrossed(string playerTag)
    {
        if (_gameEnded) { Winner = "Draw!"; return; }
        _gameEnded = true;
        Winner = (playerTag == "Player") ? "Player 1 Wins!" : "Player 2 Wins!";
        StartCoroutine(EndGame());
    }

    public void OnPlayerEliminated(string eliminatedTag)
    {
        if (_gameEnded) return;
        _gameEnded = true;
        Winner = (eliminatedTag == "Player") ? "Player 2 Wins!" : "Player 1 Wins!";
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        P1Score = scoreManager1 != null ? scoreManager1.CurrentScore : 0;
        P2Score = scoreManager2 != null ? scoreManager2.CurrentScore : 0;

        if (Winner.StartsWith("Player 1"))
            PlayerPrefs.SetInt("PvP_P1Wins", PlayerPrefs.GetInt("PvP_P1Wins", 0) + 1);
        else if (Winner.StartsWith("Player 2"))
            PlayerPrefs.SetInt("PvP_P2Wins", PlayerPrefs.GetInt("PvP_P2Wins", 0) + 1);
        PlayerPrefs.SetInt("PvP_TotalMatches", PlayerPrefs.GetInt("PvP_TotalMatches", 0) + 1);
        PlayerPrefs.Save();

        FreezePlayer(player1);
        FreezePlayer(player2);

        yield return new WaitForSeconds(0.5f);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadSceneWithFade("PvPSummary");
        }
        else
        {
            SceneManager.LoadScene("PvPSummary");
        }
    }

    private static void FreezePlayer(GameObject player)
    {
        if (player == null) return;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    private void PauseGame()
    {
        _paused = true;
        Time.timeScale = 0f;
        var pausePanel = GameObject.Find("Canvas_Pause")?.transform.Find("PausePanel")?.gameObject;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        _paused = false;
        Time.timeScale = 1f;
        var pausePanel = GameObject.Find("Canvas_Pause")?.transform.Find("PausePanel")?.gameObject;
        if (pausePanel != null) pausePanel.SetActive(false);
    }
}
