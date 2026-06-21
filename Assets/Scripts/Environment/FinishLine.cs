using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Placed on the finish-line trigger collider.
/// Solo mode: loads ScoreSummary.
/// PvP mode: notifies PvPGameManager of the winning player.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FinishLine : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   finishClip;

    [Header("Mode")]
    [SerializeField] private bool isPvP = false;

    private bool _triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;

        bool isPlayer  = other.CompareTag("Player");
        bool isPlayer2 = other.CompareTag("Player2");

        if (!isPlayer && !isPlayer2) return;

        _triggered = true;

        // Play finish SFX
        if (audioSource != null && finishClip != null)
            audioSource.PlayOneShot(finishClip, 1.0f);

        if (isPvP)
        {
            string tag = isPlayer ? "Player" : "Player2";
            PvPGameManager.Instance?.OnFinishLineCrossed(tag);
        }
        else
        {
            // Save score for summary screen
            var scoreManager = other.GetComponentInParent<ScoreManager>();
            if (scoreManager != null)
                PlayerPrefs.SetInt("LastScore", scoreManager.CurrentScore);
            PlayerPrefs.SetInt("TotalRuns", PlayerPrefs.GetInt("TotalRuns", 0) + 1);
            PlayerPrefs.Save();

            Time.timeScale = 0f;
            SceneManager.LoadScene("ScoreSummary");
        }
    }
}
