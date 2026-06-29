using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Mode Select screen — shown after clicking Start on the Main Menu.
/// Lets the player choose Solo or PvP.
/// Self-heals: auto-finds buttons by name if Inspector references are null.
/// </summary>
public class ModeSelectManager : MonoBehaviour
{
    [SerializeField] private Button soloButton;
    [SerializeField] private Button pvpButton;
    [SerializeField] private Button backButton;

    private void Start()
    {
        if (soloButton == null) soloButton = FindBtn("Solo_Btn");
        if (pvpButton  == null) pvpButton  = FindBtn("PvP_Btn");
        if (backButton == null) backButton = FindBtn("Back_Btn");

        if (soloButton != null) soloButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null) GameManager.Instance.LoadSceneWithFade("Level1");
            else SceneManager.LoadScene("Level1");
        });
        else Debug.LogWarning("[ModeSelectManager] soloButton not found!");

        if (pvpButton != null) pvpButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null) GameManager.Instance.LoadSceneWithFade("Level1_PvP");
            else SceneManager.LoadScene("Level1_PvP");
        });
        else Debug.LogWarning("[ModeSelectManager] pvpButton not found!");

        if (backButton != null) backButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null) GameManager.Instance.LoadSceneWithFade("MainMenu");
            else SceneManager.LoadScene("MainMenu");
        });
        else Debug.LogWarning("[ModeSelectManager] backButton not found!");

        // Ensure EventSystem
        if (FindObjectsByType<EventSystem>(FindObjectsInactive.Include).Length == 0)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    static Button FindBtn(string name) => GameObject.Find(name)?.GetComponent<Button>();
}
