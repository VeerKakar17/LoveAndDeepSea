using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueEvent : MonoBehaviour
{
    public string dialogueText;
    private int currCharacter;
    private float timer;
    private const float TIME_BETWEEN_CHARS = 0.025f;
    private string speaker;
    private DialogueManager.SpeakerSprite sprite;

    public DialogueEvent(string dialogueText, string speaker, DialogueManager.SpeakerSprite sprite) : base()
    {
        this.dialogueText = dialogueText;
        this.speaker = speaker;
        this.sprite = sprite;
    }

    public void StartEvent()
    {
        currCharacter = 0;
        timer = 0;
        Debug.Log("Dialogue: " + dialogueText);
    }

    public void UpdateEvent()
    {
        timer += Time.deltaTime;
        if (currCharacter < dialogueText.Length && TIME_BETWEEN_CHARS <= timer)
        {
            timer -= TIME_BETWEEN_CHARS;
            currCharacter++;

            if (currCharacter == dialogueText.Length)
            {
                GameManager.instance.dialogueManager.SetDialogue(dialogueText, speaker, sprite, true);
            }
            else
            {
                GameManager.instance.dialogueManager.SetDialogue(dialogueText.Substring(0, currCharacter), speaker, sprite);
            }
        }

        // detect input (skip dialogue)
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            if (currCharacter < dialogueText.Length)
            {
                timer = 0;
                currCharacter = dialogueText.Length;
                GameManager.instance.dialogueManager.SetDialogue(dialogueText, speaker, sprite, true);
            }
            else if (timer > 0.1f)
            {
                EndEvent();
            }
        }
    }

    public void EndEvent()
    {
        GameManager.instance.dialogueManager.ClearDialogue();
        GameManager.instance.dialogueManager.StartNextEvent();
    }

}