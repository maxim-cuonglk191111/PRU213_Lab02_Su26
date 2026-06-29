using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton — owns Solo mode game state.
/// States: MainMenu | Playing | Paused | GameOver
/// </summary>
public class GameManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── State Enum ─────────────────────────────────────────────
    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState State { get; private set; } = GameState.MainMenu;

    // ── Unity Lifecycle ────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Reset playing state whenever Level1 (re)loads so Escape works after Replay
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            State = GameState.Playing;
            Time.timeScale = 1f;
            SetPausePanel(false);
        }
    }

    private void Start()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
    }

    // ── Public API ─────────────────────────────────────────────
    public void PauseGame()
    {
        if (State != GameState.Playing) return;
        State = GameState.Paused;
        Time.timeScale = 0f;
        SetPausePanel(true);
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        State = GameState.Playing;
        Time.timeScale = 1f;
        SetPausePanel(false);
    }

    public void GameOver()
    {
        State = GameState.GameOver;
        Time.timeScale = 0f;

        // Save score before leaving the gameplay scene
        var sm = Object.FindFirstObjectByType<ScoreManager>();
        if (sm != null)
        {
            PlayerPrefs.SetInt("LastScore", sm.CurrentScore);
            PlayerPrefs.SetInt("TotalRuns", PlayerPrefs.GetInt("TotalRuns", 0) + 1);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("ScoreSummary");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == GameState.Playing)  PauseGame();
            else if (State == GameState.Paused) ResumeGame();
        }
    }

    // ── Helpers ────────────────────────────────────────────────
    private static void SetPausePanel(bool active)
    {
        // PausePanel is placed in the gameplay scene by the setup script
        GameObject.Find("PausePanel")?.SetActive(active);
    }
}
