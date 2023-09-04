using UnityEngine.Events;

public class GameEvents
{
    public static readonly UnityEvent OnLevelBuiltEvent = new UnityEvent();
    public static readonly UnityEvent OnGameStartEvent = new UnityEvent();
    public static readonly UnityEvent<bool> OnGameOverEvent = new UnityEvent<bool>();
    public static readonly UnityEvent OnPlayerCollectedCoinEvent = new UnityEvent();
    public static readonly UnityEvent<PlayerController> OnPlayerLoseEvent = new UnityEvent<PlayerController>();

    public static void InvokeLevelBuiltEvent()
    {
        OnLevelBuiltEvent.Invoke();
    }

    public static void InvokeGameStartEvent()
    {
        OnGameStartEvent.Invoke();
    }

    public static void InvokeGameOverEvent(bool isPlayerVictory = false)
    {
        OnGameOverEvent.Invoke(isPlayerVictory);
    }

    public static void InvokePlayerCollectedCoinEvent()
    {
        OnPlayerCollectedCoinEvent.Invoke();
    }

    public static void InvokePlayerLoseEvent(PlayerController player)
    {
        OnPlayerLoseEvent.Invoke(player);
    }
}
