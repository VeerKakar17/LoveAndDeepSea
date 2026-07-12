using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] public Player player;
    public DialogueManager dialogueManager;
    public static GameManager instance;
    public static float TIME_Y_OFFSET = 10000f;
    public bool inDialogueState = false;

    public enum Time
    {
        ModernTime,
        AncientTime
    }
// time managment
    [SerializeField] public float maxAncientTime = 60f; 

    public float RemainingAncientTime { get; private set; }
    
    public static event Action<float> OnAncientTimeChanged;

    private CameraShake shake;
//

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
        shake = FindFirstObjectByType<CameraShake>();
    }

    private void Start()
    {
        RemainingAncientTime = maxAncientTime;
        OnAncientTimeChanged?.Invoke(RemainingAncientTime);

        // material.SetVector(
        //     "_Center",
        //     Camera.main.WorldToViewportPoint(player.position)
        // );

        // material.SetFloat(
        //     "_Radius",
        //     radius
        // );
    }

    private void Update()
    {
        if (inDialogueState)
        {
            dialogueManager.OnUpdate();
            return;
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            StartDialogueState();
        }

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
        StartCoroutine(SwapSequence());
    }

    IEnumerator SwapSequence()
    {
        // if (CurrentTime == Time.ModernTime)
        // {
        //     transition.material.SetFloat("_SwapDirection", 0f);
        // }
        // else
        // {
        //     transition.material.SetFloat("_SwapDirection", 1f);
        // }
        // yield return StartCoroutine(
        //     transition.Play()
        // );
        yield return StartCoroutine(shake.Shake(.1f,.5f));

        CurrentTime =
            CurrentTime == Time.ModernTime
            ? Time.AncientTime
            : Time.ModernTime;


        OnTimeSwap?.Invoke(CurrentTime);
    }

    private void CheckLoseCondition()
    {
        if (!player.HasTreasure)
        {
            Debug.Log("lose");
        } else {
            Debug.Log("win");
        }
    }

    public void StartDialogueState()
    {
        inDialogueState = true;
        dialogueManager.StartTestDialogue();
    }

    public void EndDialogueState()
    {
        inDialogueState = false;
    }
}