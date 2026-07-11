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
// time managment
    [SerializeField] public float maxAncientTime = 90f; // 3 minutes // actually changing to 1min30sec

    public float RemainingAncientTime { get; private set; }
    
    public static event Action<float> OnAncientTimeChanged;
// time managment

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

    private void Start()
    {
        RemainingAncientTime = maxAncientTime;
        OnAncientTimeChanged?.Invoke(RemainingAncientTime);
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            DoTimeSwap();
        }
        
        if (CurrentTime == Time.AncientTime)
        {
            RemainingAncientTime -= UnityEngine.Time.deltaTime;
            RemainingAncientTime = Mathf.Max(0, RemainingAncientTime);

            OnAncientTimeChanged?.Invoke(RemainingAncientTime);

            if (RemainingAncientTime <= 0)
            {
                CheckLoseCondition();
            }
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

    private void CheckLoseCondition()
    {
        // if (!PlayerInventory.instance.HasTreasure())
        // {
        //     Debug.Log("Game Over!");
        // }
    }
}