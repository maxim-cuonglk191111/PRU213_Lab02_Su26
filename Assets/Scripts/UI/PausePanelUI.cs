using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelUI : MonoBehaviour
{
    public void Resume()
    {
        if (GameManager.Instance != null) GameManager.Instance.ResumeGame();
        else if (PvPGameManager.Instance != null) PvPGameManager.Instance.ResumeGame();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadSceneWithFade("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
