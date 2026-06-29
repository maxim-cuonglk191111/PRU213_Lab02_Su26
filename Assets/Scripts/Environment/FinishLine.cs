using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class FinishLine : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   finishClip;

    [Header("Mode")]
    [SerializeField] private bool isPvP = false;

    bool triggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        bool isP1 = other.CompareTag("Player");
        bool isP2 = other.CompareTag("Player2");
        if (!isP1 && !isP2) return;

        triggered = true;

        // Stop the player
        var pc = other.GetComponentInParent<PlayerController>() ?? other.GetComponent<PlayerController>();
        pc?.SetFinished();

        // Play finish SFX
        if (audioSource != null && finishClip != null)
            audioSource.PlayOneShot(finishClip);
        else
            AudioManager.Instance?.PlayFinishSound();

        if (isPvP)
        {
            PvPGameManager.Instance?.OnFinishLineCrossed(isP1 ? "Player" : "Player2");
        }
        else
        {
            var sm = other.GetComponentInParent<ScoreManager>() ?? other.GetComponent<ScoreManager>();
            if (sm != null)
            {
                PlayerPrefs.SetInt("LastScore", sm.CurrentScore);
                PlayerPrefs.SetInt("TotalRuns", PlayerPrefs.GetInt("TotalRuns", 0) + 1);
                PlayerPrefs.Save();
            }

            if (GameManager.Instance != null)
                Invoke(nameof(TriggerFinish), 0.5f);
            else
            {
                Time.timeScale = 0f;
                SceneManager.LoadScene("ScoreSummary");
            }
        }
    }

    void TriggerFinish() => GameManager.Instance?.LevelComplete();

    void OnEnable() { triggered = false; }
}
