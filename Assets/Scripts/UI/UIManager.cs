using UnityEngine;

// Stub — satisfies compile references from reference scripts.
// Our project uses HUDManager + PausePanelUI instead of UIManager.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowGameOverPanel()      { }
    public void ShowLevelCompletePanel() { }
    public void ShowPauseMenu()          { }
    public void HidePauseMenu()          { }
    public void ShowFloatingText(string text, Vector3 position) { }
    public void ShowNotification(string text) { }
}
