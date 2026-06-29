using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState State { get; private set; } = GameState.MainMenu;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            State = GameState.Playing;
            Time.timeScale = 1f;
            SetPausePanel(false);
            PurgePvPRemnants();
        }
    }

    private static void PurgePvPRemnants()
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("Player2"))
            Destroy(go);
        var strayP2 = GameObject.Find("Player2");
        if (strayP2 != null) Destroy(strayP2);
        var pvpGM = FindAnyObjectByType<PvPGameManager>();
        if (pvpGM != null) Destroy(pvpGM.gameObject);
        var cam2 = GameObject.Find("Camera2");
        if (cam2 != null) Destroy(cam2);
        var divider = GameObject.Find("Canvas_Divider");
        if (divider != null) Destroy(divider);
    }

    private void Start()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
    }

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
        var sm = Object.FindAnyObjectByType<ScoreManager>();
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
            if (State == GameState.Playing) PauseGame();
            else if (State == GameState.Paused) ResumeGame();
        }
    }

    private static void SetPausePanel(bool active)
    {
        GameObject.Find("PausePanel")?.SetActive(active);
    }
}
