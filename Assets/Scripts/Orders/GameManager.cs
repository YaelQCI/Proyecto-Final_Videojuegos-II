using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { WaitingToStart, Playing, GameOver, Victory }

    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.WaitingToStart;

    public System.Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartGame() => SetState(GameState.Playing);

    public void SetState(GameState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(State);
    }
}
