using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Main Menu controller — Solo/PvP button now redirects through ModeSelect.
/// Self-heals: auto-finds buttons by name if Inspector references are null.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider     sfxSlider;
    [SerializeField] private Slider     musicSlider;

    private void Awake()
    {
        EnsureEventSystem();
    }

    private void Start()
    {
        // Auto-find buttons by name if not assigned in Inspector
        if (startButton   == null) startButton   = FindButtonByName("Start_Btn");
        if (optionsButton == null) optionsButton = FindButtonByName("Options_Btn");
        if (quitButton    == null) quitButton    = FindButtonByName("Quit_Btn");
        if (optionsPanel  == null) optionsPanel  = GameObject.Find("OptionsPanel");

        // Wire listeners
        if (startButton   != null) startButton.onClick.AddListener(OnStartClicked);
        else Debug.LogWarning("[MainMenuManager] startButton not found!");

        if (optionsButton != null) optionsButton.onClick.AddListener(OnOptionsClicked);
        else Debug.LogWarning("[MainMenuManager] optionsButton not found!");

        if (quitButton    != null) quitButton.onClick.AddListener(OnQuitClicked);
        else Debug.LogWarning("[MainMenuManager] quitButton not found!");

        if (optionsPanel  != null) optionsPanel.SetActive(false);

        // Sync sliders to current volume
        if (AudioManager.Instance != null)
        {
            if (sfxSlider   != null) { sfxSlider.value   = AudioManager.Instance.SFXVolume;   sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance.SFXVolume = v); }
            if (musicSlider != null) { musicSlider.value = AudioManager.Instance.MusicVolume; musicSlider.onValueChanged.AddListener(v => AudioManager.Instance.MusicVolume = v); }
        }

        Debug.Log("[MainMenuManager] Initialized. Start=" + (startButton != null) +
                  " Options=" + (optionsButton != null) +
                  " Quit=" + (quitButton != null));
    }

    private void OnStartClicked()
    {
        Debug.Log("[MainMenuManager] Start clicked!");
        SceneManager.LoadScene("ModeSelect");
    }
    private void OnOptionsClicked()
    {
        Debug.Log("[MainMenuManager] Options clicked!");
        if (optionsPanel != null) optionsPanel.SetActive(!optionsPanel.activeSelf);
    }
    private void OnQuitClicked()
    {
        Debug.Log("[MainMenuManager] Quit clicked!");
        Application.Quit();
    }

    // ── Helpers ────────────────────────────────────────────────
    static Button FindButtonByName(string name)
    {
        var go = GameObject.Find(name);
        if (go == null) return null;
        return go.GetComponent<Button>();
    }

    /// <summary>Ensures an EventSystem exists — without it no button click registers.</summary>
    static void EnsureEventSystem()
    {
        if (FindObjectsByType<EventSystem>(FindObjectsInactive.Include).Length == 0)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
            Debug.LogWarning("[MainMenuManager] EventSystem was missing — created one at runtime.");
        }
    }
}
