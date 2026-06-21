using System;
using UnityEngine;

/// <summary>
/// Non-singleton score tracker — one instance per player.
/// Score = Σ(base_points × current_multiplier).
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // ── Events ─────────────────────────────────────────────────
    /// <summary>Fired whenever the score changes. Passes the new score.</summary>
    public event Action<int> OnScoreChanged;

    // ── State ──────────────────────────────────────────────────
    public int CurrentScore  { get; private set; }
    public int CurrentMultiplier { get; private set; } = 1;

    private const int MultiplierCap = 4;

    // ── Public API ─────────────────────────────────────────────
    /// <summary>Add base points multiplied by the current multiplier.</summary>
    public void AddScore(int basePoints)
    {
        CurrentScore += basePoints * CurrentMultiplier;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    /// <summary>Increment multiplier by 1 (capped at ×4).</summary>
    public void IncrementMultiplier()
    {
        CurrentMultiplier = Mathf.Min(CurrentMultiplier + 1, MultiplierCap);
    }

    /// <summary>Reset multiplier to ×1 (called on crash).</summary>
    public void ResetMultiplier()
    {
        CurrentMultiplier = 1;
    }

    /// <summary>Reset score and multiplier to zero/one.</summary>
    public void ResetScore()
    {
        CurrentScore     = 0;
        CurrentMultiplier = 1;
        OnScoreChanged?.Invoke(CurrentScore);
    }
}
