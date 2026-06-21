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
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        State = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        State = GameState.GameOver;
        Time.timeScale = 0f;
        SceneManager.LoadScene("ScoreSummary");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == GameState.Playing) PauseGame();
            else if (State == GameState.Paused) ResumeGame();
        }
    }
}
