using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Panels")]
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider     sfxSlider;
    [SerializeField] private Slider     musicSlider;

    private void Awake()
    {
        EnsureEventSystem();
    }

    private void Start()
    {
        if (startButton     == null) startButton     = FindBtn("Start_Btn");
        if (howToPlayButton == null) howToPlayButton = FindBtn("HowToPlay_Btn");
        if (optionsButton   == null) optionsButton   = FindBtn("Options_Btn");
        if (quitButton      == null) quitButton      = FindBtn("Quit_Btn");
        if (guidePanel      == null) guidePanel      = GameObject.Find("GuidePanel");
        if (optionsPanel    == null) optionsPanel    = GameObject.Find("OptionsPanel");

        if (startButton     != null) startButton.onClick.AddListener(() => SceneManager.LoadScene("ModeSelect"));
        if (howToPlayButton != null) howToPlayButton.onClick.AddListener(ToggleGuide);
        if (optionsButton   != null) optionsButton.onClick.AddListener(() => {
            if (optionsPanel != null) optionsPanel.SetActive(!optionsPanel.activeSelf);
            if (guidePanel   != null) guidePanel.SetActive(false);
        });
        if (quitButton != null) quitButton.onClick.AddListener(Application.Quit);

        // Wire the Close button inside GuidePanel
        var closeBtn = FindBtn("Close_Btn");
        if (closeBtn != null) closeBtn.onClick.AddListener(() => { if (guidePanel != null) guidePanel.SetActive(false); });

        if (guidePanel   != null) guidePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        if (AudioManager.Instance != null)
        {
            if (sfxSlider   != null) { sfxSlider.value   = AudioManager.Instance.SFXVolume;   sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance.SFXVolume = v); }
            if (musicSlider != null) { musicSlider.value = AudioManager.Instance.MusicVolume; musicSlider.onValueChanged.AddListener(v => AudioManager.Instance.MusicVolume = v); }
        }
    }

    private void ToggleGuide()
    {
        if (guidePanel == null) return;
        guidePanel.SetActive(!guidePanel.activeSelf);
        if (guidePanel.activeSelf && optionsPanel != null) optionsPanel.SetActive(false);
    }

    static Button FindBtn(string name) => GameObject.Find(name)?.GetComponent<Button>();

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
