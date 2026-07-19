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
    private bool finalDialogue = false;

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
    
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip timeSwapSFX;
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

        inDialogueState = true;
        dialogueManager.StartIntroDialogue();

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
        sfxSource.PlayOneShot(timeSwapSFX);
        
        shake.enableBounds = false;
        if (CurrentTime == Time.ModernTime)
        {
            // transition.material.SetFloat("_SwapDirection", 0f);
            shake.minBounds.y += 10000f;
            shake.maxBounds.y += 10000f;
        }
        else
        {
            // transition.material.SetFloat("_SwapDirection", 1f);
            shake.minBounds.y -= 10000f;
            shake.maxBounds.y -= 10000f;
        }
        StartCoroutine(SwapSequence());

    }

    IEnumerator SwapSequence()
    {
        
        // yield return StartCoroutine(
        //     transition.Play()
        // );
        yield return StartCoroutine(shake.Shake(.1f,.5f));

        

        CurrentTime =
            CurrentTime == Time.ModernTime
            ? Time.AncientTime
            : Time.ModernTime;


        OnTimeSwap?.Invoke(CurrentTime);

        shake.enableBounds = true;
    }

    private void CheckLoseCondition()
    {
        inDialogueState = true;
        finalDialogue = true;
        if (!player.HasTreasure)
        {
            Debug.Log("lose");
            dialogueManager.StartBadEnding();
        } else {
            Debug.Log("win");
            dialogueManager.StartGoodEndingDialogue();
        }
    }

    public void StartDialogueState()
    {
        inDialogueState = true;
        dialogueManager.StartTestDialogue();
    }

    public void EndDialogueState()
    {
        if (!finalDialogue)
        {
            inDialogueState = false;
        }
    }
}