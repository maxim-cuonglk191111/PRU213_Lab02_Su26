using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Mode Select screen — shown after clicking Start on the Main Menu.
/// Lets the player choose Solo or PvP.
/// </summary>
public class ModeSelectManager : MonoBehaviour
{
    [SerializeField] private Button soloButton;
    [SerializeField] private Button pvpButton;
    [SerializeField] private Button backButton;

    private void Start()
    {
        if (soloButton != null) soloButton.onClick.AddListener(() => SceneManager.LoadScene("Level1"));
        if (pvpButton  != null) pvpButton.onClick.AddListener(()  => SceneManager.LoadScene("Level1_PvP"));
        if (backButton != null) backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }
}
