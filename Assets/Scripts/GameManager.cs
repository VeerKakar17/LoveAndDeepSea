using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static float TIME_Y_OFFSET = 10000f;

    public enum Time
    {
        ModernTime,
        AncientTime
    }

    public Time CurrentTime { get; private set; } = Time.ModernTime;

    public static event Action<Time> OnTimeSwap;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            DoTimeSwap();
        }
    }

    public void DoTimeSwap()
    {
        Debug.Log("Swapping");
        CurrentTime = CurrentTime == Time.ModernTime
                ? Time.AncientTime
                : Time.ModernTime;

        OnTimeSwap?.Invoke(CurrentTime);
    }
}