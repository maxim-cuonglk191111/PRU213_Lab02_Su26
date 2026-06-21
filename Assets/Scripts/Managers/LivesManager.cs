using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Non-singleton lives tracker — one instance per player.
/// Notifies LivesChanged event and delegates game-over/elimination to GameManager or PvPGameManager.
/// </summary>
public class LivesManager : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────
    [SerializeField] private int startingLives = 3;

    /// <summary>
    /// If set, this player is eliminated when lives reach 0 (PvP mode).
    /// PvPGameManager subscribes to OnPlayerEliminated.
    /// </summary>
    [SerializeField] private string playerTag = "Player";

    // ── Events ─────────────────────────────────────────────────
    public event Action<int> OnLivesChanged;
    public event Action<string> OnPlayerEliminated;  // passes playerTag

    // ── State ──────────────────────────────────────────────────
    public int CurrentLives { get; private set; }

    private void Awake()
    {
        CurrentLives = startingLives;
    }

    // ── Public API ─────────────────────────────────────────────
    public void LoseLife()
    {
        CurrentLives = Mathf.Max(0, CurrentLives - 1);
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives == 0)
        {
            OnPlayerEliminated?.Invoke(playerTag);

            // Only trigger Solo game-over if PvP is not handling it
            if (PvPGameManager.Instance == null && GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
    }

    public void ResetLives()
    {
        CurrentLives = startingLives;
        OnLivesChanged?.Invoke(CurrentLives);
    }
}
