using System;
using UnityEngine;

public class LivesManager : MonoBehaviour
{
    [SerializeField] private int startingLives = 3;
    [SerializeField] private string playerTag = "Player";

    public event Action<int> OnLivesChanged;
    public event Action<string> OnPlayerEliminated;

    public int CurrentLives { get; private set; }

    private void Awake()
    {
        CurrentLives = startingLives;
    }

    public void LoseLife()
    {
        CurrentLives = Mathf.Max(0, CurrentLives - 1);
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives == 0)
        {
            OnPlayerEliminated?.Invoke(playerTag);
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
