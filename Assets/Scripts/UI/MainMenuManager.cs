using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main Menu controller — Solo/PvP button now redirects through ModeSelect.
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

    private void Start()
    {
        if (startButton   != null) startButton.onClick.AddListener(OnStartClicked);
        if (optionsButton != null) optionsButton.onClick.AddListener(OnOptionsClicked);
        if (quitButton    != null) quitButton.onClick.AddListener(OnQuitClicked);

        if (optionsPanel  != null) optionsPanel.SetActive(false);

        // Sync sliders to current volume
        if (AudioManager.Instance != null)
        {
            if (sfxSlider   != null) { sfxSlider.value   = AudioManager.Instance.SFXVolume;   sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance.SFXVolume = v); }
            if (musicSlider != null) { musicSlider.value = AudioManager.Instance.MusicVolume; musicSlider.onValueChanged.AddListener(v => AudioManager.Instance.MusicVolume = v); }
        }
    }

    private void OnStartClicked()   => SceneManager.LoadScene("ModeSelect");
    private void OnOptionsClicked() { if (optionsPanel != null) optionsPanel.SetActive(!optionsPanel.activeSelf); }
    private void OnQuitClicked()    => Application.Quit();
}
