using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Placed on the PausePanel GameObject.
/// Provides Resume and Main Menu callbacks for the pause overlay buttons.
/// Works in both Solo (GameManager) and PvP (PvPGameManager) scenes.
/// </summary>
public class PausePanelUI : MonoBehaviour
{
    public void Resume()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
        else if (PvPGameManager.Instance != null)
            PvPGameManager.Instance.ResumeGame();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
