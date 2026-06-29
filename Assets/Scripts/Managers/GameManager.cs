using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, Paused, GameOver, LevelComplete }
    public GameState State { get; private set; }

    [Header("Game Settings")]
    public int startingLives = 1;
    public string level1Scene = "Level1";
    public string level2Scene = "Level2";
    public string level3Scene = "Level3";
    public string endScreenScene = "ScoreSummary";
    public float restartDelay = 2f;

    public int Lives { get; private set; }

    CanvasGroup fadeGroup;

    void Awake()
    {
        endScreenScene = "ScoreSummary";

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            CreateFadeCanvas();

            if (AudioManager.Instance == null)
            {
                GameObject audioManagerGO = new GameObject("AudioManager");
                audioManagerGO.AddComponent<AudioManager>();
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Lives = startingLives;
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "Menu" || scene == "MainMenu" || scene == "ModeSelect")
        {
            State = GameState.Menu;
        }
        else
        {
            State = GameState.Playing;
        }
    }

    void CreateFadeCanvas()
    {
        GameObject fadeObj = new GameObject("FadeCanvas");
        fadeObj.transform.SetParent(this.transform);

        Canvas canvas = fadeObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        fadeObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        fadeObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(fadeObj.transform, false);

        UnityEngine.UI.Image img = imgObj.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.black;

        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        fadeGroup = fadeObj.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
        fadeGroup.interactable = false;
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName, -1));
    }

    public void LoadSceneWithFade(int buildIndex)
    {
        StartCoroutine(FadeAndLoad(string.Empty, buildIndex));
    }

    private System.Collections.IEnumerator FadeAndLoad(string sceneName, int buildIndex)
    {
        Time.timeScale = 1f;
        if (fadeGroup != null)
        {
            fadeGroup.blocksRaycasts = true;
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.unscaledDeltaTime;
                fadeGroup.alpha = Mathf.Clamp01(t / 0.5f);
                yield return null;
            }
            fadeGroup.alpha = 1f;
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            yield return null;
        }
        else
        {
            SceneManager.LoadScene(buildIndex);
            yield return null;
        }

        if (fadeGroup != null)
        {
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.unscaledDeltaTime;
                fadeGroup.alpha = 1f - Mathf.Clamp01(t / 0.5f);
                yield return null;
            }
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    Rigidbody2D playerRb;
    float startXPos;
    PlayerController playerController;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu" || scene.name == "MainMenu")
        {
            State = GameState.Menu;
        }
        else if (scene.name == level1Scene || scene.name == "Level1")
        {
            State = GameState.Playing;
            Time.timeScale = 1f;

            // Remove any PvP remnants when loading solo level
            foreach (var go in GameObject.FindGameObjectsWithTag("Player2"))
                Destroy(go);
            var strayP2 = GameObject.Find("Player2");
            if (strayP2 != null) Destroy(strayP2);
            var pvpGM = FindAnyObjectByType<PvPGameManager>();
            if (pvpGM != null) Destroy(pvpGM.gameObject);
            var cam2 = GameObject.Find("Camera2");
            if (cam2 != null) Destroy(cam2);

            SpawnItemsProcedurally();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerController>();
                startXPos = player.transform.position.x;

                float targetDistance = 1000f;
                FinishLine finishLine = Object.FindAnyObjectByType<FinishLine>();
                if (finishLine != null)
                {
                    float targetX = startXPos + targetDistance;
                    RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(targetX, 500f), Vector2.down, 1000f);
                    foreach (var hit in hits)
                    {
                        if (hit.collider != null && hit.collider.CompareTag("Ground"))
                        {
                            finishLine.transform.position = new Vector3(targetX, hit.point.y + 4f, 0f);
                            break;
                        }
                    }
                }
            }
        }
        else if (scene.name == "Level1_PvP")
        {
            State = GameState.Playing;
            Time.timeScale = 1f;
        }
    }

    void SpawnItemsProcedurally()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Items");
        if (prefabs == null || prefabs.Length == 0) return;

        GameObject itemsParent = GameObject.Find("--- ITEMS ---");
        if (itemsParent == null)
        {
            itemsParent = new GameObject("--- ITEMS ---");
        }
        else
        {
            foreach (Transform child in itemsParent.transform)
                Destroy(child.gameObject);
        }

        EdgeCollider2D groundEdge = Object.FindAnyObjectByType<EdgeCollider2D>();
        if (groundEdge != null)
        {
            Vector2[] points = groundEdge.points;
            Transform groundTransform = groundEdge.transform;

            int spawnInterval = 15;
            for (int i = 5; i < points.Length - 5; i += spawnInterval)
            {
                i += Random.Range(-3, 4);
                if (i < 0 || i >= points.Length) continue;

                Vector2 worldPos = groundTransform.TransformPoint(points[i]);
                worldPos.y += Random.Range(1.5f, 4.0f);

                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                Instantiate(randomPrefab, worldPos, Quaternion.identity).transform.SetParent(itemsParent.transform);
            }
        }
    }

    void Update()
    {
        if (State == GameState.Playing)
        {
            if (playerRb == null || playerController == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerRb = player.GetComponent<Rigidbody2D>();
                    playerController = player.GetComponent<PlayerController>();
                    startXPos = player.transform.position.x;
                }
            }
        }

        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame ||
                UnityEngine.InputSystem.Keyboard.current.pKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }
    }

    public void GameOver() => PlayerCrashed();

    public void PlayerCrashed()
    {
        if (State == GameState.LevelComplete || State == GameState.GameOver) return;
        Lives = 0;
        State = GameState.GameOver;

        if (ScoreManager.Instance != null)
        {
            PlayerPrefs.SetInt("LastScore", ScoreManager.Instance.CurrentScore);
            PlayerPrefs.SetInt("TotalRuns", PlayerPrefs.GetInt("TotalRuns", 0) + 1);
            PlayerPrefs.Save();
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOverPanel();
        else
            LoadSceneWithFade("ScoreSummary");
    }

    public void LevelComplete(bool playSFX = true)
    {
        if (State == GameState.GameOver || State == GameState.LevelComplete) return;
        State = GameState.LevelComplete;

        if (playSFX && AudioManager.Instance != null)
            AudioManager.Instance.PlayFinishSound();

        if (ScoreManager.Instance != null)
        {
            PlayerPrefs.SetInt("LastScore", ScoreManager.Instance.CurrentScore);
            PlayerPrefs.SetInt("TotalRuns", PlayerPrefs.GetInt("TotalRuns", 0) + 1);
            PlayerPrefs.Save();
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowLevelCompletePanel();
        else
            LoadSceneWithFade("ScoreSummary");
    }

    public void TogglePause()
    {
        if (State == GameState.Playing)
        {
            State = GameState.Paused;
            Time.timeScale = 0f;
            if (UIManager.Instance != null)
                UIManager.Instance.ShowPauseMenu();
            else
                GameObject.Find("PausePanel")?.SetActive(true);
        }
        else if (State == GameState.Paused)
        {
            State = GameState.Playing;
            Time.timeScale = 1f;
            if (UIManager.Instance != null)
                UIManager.Instance.HidePauseMenu();
            else
                GameObject.Find("PausePanel")?.SetActive(false);
        }
    }

    public void StartGame()
    {
        Lives = startingLives;
        Time.timeScale = 1f;
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
        if (TrickManager.Instance != null) TrickManager.Instance.ResetAll();
        LoadSceneWithFade(!string.IsNullOrEmpty(level1Scene) ? level1Scene : "Level1");
    }

    public void RestartGame()
    {
        Lives = startingLives;
        Time.timeScale = 1f;
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
        if (TrickManager.Instance != null) TrickManager.Instance.ResetAll();
        LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadEndScreen()
    {
        Time.timeScale = 1f;
        LoadSceneWithFade(!string.IsNullOrEmpty(endScreenScene) ? endScreenScene : "ScoreSummary");
    }

    public void PauseGame()  => TogglePause();
    public void ResumeGame() => TogglePause();

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
