using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScoreSummaryManager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI distanceText;

    [Header("Buttons")]
    [SerializeField] private Button replayButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        EnsureEventSystem();
        Time.timeScale = 1f;

        if (currentScoreText == null) currentScoreText = FindTMP("Your_Score:_0");
        if (bestScoreText    == null) bestScoreText    = FindTMP("Best_Score:_0");
        if (timeText         == null) timeText         = FindTMP("Time:_00:00");
        if (distanceText     == null) distanceText     = FindTMP("Distance:_0m");
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

        if (currentScoreText != null) currentScoreText.text = $"Score: {current}";
        if (bestScoreText    != null) bestScoreText.text    = $"Best: {best}";

        float elapsed = PlayerPrefs.GetFloat("LastTime", 0f);
        int min = Mathf.FloorToInt(elapsed / 60f);
        int sec = Mathf.FloorToInt(elapsed % 60f);
        if (timeText != null) timeText.text = $"Time: {min:00}:{sec:00}";

        int dist = PlayerPrefs.GetInt("LastDistance", 0);
        if (distanceText != null) distanceText.text = $"Distance: {dist}m";

        if (replayButton != null) replayButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null) GameManager.Instance.LoadSceneWithFade("Level1");
            else SceneManager.LoadScene("Level1");
        });
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null) GameManager.Instance.LoadSceneWithFade("MainMenu");
            else SceneManager.LoadScene("MainMenu");
        });
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
