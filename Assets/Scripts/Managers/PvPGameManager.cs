using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PvP-only game state manager.
/// Handles win by finish line, win by elimination, and Escape pause in PvP.
/// </summary>
public class PvPGameManager : MonoBehaviour
{
    // ── Singleton-like (scene-scoped) ──────────────────────────
    public static PvPGameManager Instance { get; private set; }

    // ── Inspector ──────────────────────────────────────────────
    [Header("Player References")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [Header("Per-Player Managers")]
    [SerializeField] private LivesManager livesManager1;
    [SerializeField] private LivesManager livesManager2;
    [SerializeField] private ScoreManager scoreManager1;
    [SerializeField] private ScoreManager scoreManager2;

    // ── State ──────────────────────────────────────────────────
    public static string Winner   = "";
    public static int    P1Score  = 0;
    public static int    P2Score  = 0;

    private bool _gameEnded;
    private bool _paused;

    // ── Unity Lifecycle ────────────────────────────────────────
    private void Awake()
    {
        Instance = this;
        Winner   = "";

        if (player1 == null || player2 == null)
        {
            Debug.LogError("[PvPGameManager] Player1 or Player2 not assigned — falling back to Solo mode.");
            SceneManager.LoadScene("Level1");
            return;
        }

        // Subscribe to elimination events
        if (livesManager1 != null)
            livesManager1.OnPlayerEliminated += tag => OnPlayerEliminated(tag);
        if (livesManager2 != null)
            livesManager2.OnPlayerEliminated += tag => OnPlayerEliminated(tag);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_paused) PauseGame();
            else ResumeGame();
        }
    }

    // ── Win Handling ───────────────────────────────────────────
    /// <summary>Called by FinishLine.cs when a player crosses the line.</summary>
    public void OnFinishLineCrossed(string playerTag)
    {
        if (_gameEnded)
        {
            // Second player crossed in the same physics frame — declare Draw
            Winner = "Draw!";
            return;
        }
        _gameEnded = true;

        Winner = (playerTag == "Player") ? "Player 1 Wins!" : "Player 2 Wins!";
        StartCoroutine(EndGame());
    }

    /// <summary>Called by LivesManager when a player's lives reach zero.</summary>
    public void OnPlayerEliminated(string eliminatedTag)
    {
        if (_gameEnded) return;
        _gameEnded = true;

        Winner = (eliminatedTag == "Player") ? "Player 2 Wins!" : "Player 1 Wins!";
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        // Record scores
        P1Score = scoreManager1 != null ? scoreManager1.CurrentScore : 0;
        P2Score = scoreManager2 != null ? scoreManager2.CurrentScore : 0;

        // Update lifetime stats
        if (Winner.StartsWith("Player 1"))
            PlayerPrefs.SetInt("PvP_P1Wins", PlayerPrefs.GetInt("PvP_P1Wins", 0) + 1);
        else if (Winner.StartsWith("Player 2"))
            PlayerPrefs.SetInt("PvP_P2Wins", PlayerPrefs.GetInt("PvP_P2Wins", 0) + 1);
        PlayerPrefs.SetInt("PvP_TotalMatches", PlayerPrefs.GetInt("PvP_TotalMatches", 0) + 1);
        PlayerPrefs.Save();

        // Freeze both players
        FreezePlayer(player1);
        FreezePlayer(player2);

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("PvPSummary");
    }

    private static void FreezePlayer(GameObject player)
    {
        if (player == null) return;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    // ── Pause ──────────────────────────────────────────────────
    private void PauseGame()
    {
        _paused = true;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        _paused = false;
        Time.timeScale = 1f;
    }
}
