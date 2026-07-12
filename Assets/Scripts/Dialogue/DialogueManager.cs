using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Rendering.MaterialUpgrader;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private GameObject pressZIndicator;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Image speakerImage;
    [SerializeField] private List<Sprite> speakerSprites;
    private Coroutine pressZCoroutine = null;

    List<DialogueEvent> events;
    DialogueEvent currentEvent;

    public enum SpeakerSprite
    {
        MC = 0,
        DragonKing = 1,
    }

    private void Start()
    {
        events = new List<DialogueEvent>();
        currentEvent = null;
        dialogueBox.SetActive(false);
    }

    public void SetDialogue(string dialogue, string speaker, SpeakerSprite sprite, bool addPressZ = false)
    {
        TextMeshProUGUI curr;
        curr = dialogueText;
        curr.text = dialogue;
        speakerText.text = speaker;
        dialogueBox.SetActive(true);

        int spriteIndex = (int)sprite;

        if (spriteIndex >= 0 && spriteIndex < speakerSprites.Count)
        {
            speakerImage.sprite = speakerSprites[spriteIndex];
            speakerImage.SetNativeSize();
            speakerImage.gameObject.SetActive(true);
        }
        else
        {
            speakerImage.gameObject.SetActive(false);
            Debug.LogWarning("No speaker sprite assigned for " + sprite);
        }

        curr.ForceMeshUpdate();
        if (addPressZ)
        {
            pressZIndicator.SetActive(true);

            pressZCoroutine = StartCoroutine(PressZCoroutine());
        }
    }

    public void ClearDialogue()
    {
        Debug.Log("test");
        dialogueText.text = "";
        speakerText.text = "";
        pressZIndicator.SetActive(false);
        speakerImage.sprite = null;
        speakerImage.gameObject.SetActive(false);
        dialogueBox.SetActive(false);
        if (pressZCoroutine != null)
        {
            StopCoroutine(pressZCoroutine);
            pressZCoroutine = null;
        }
    }

    private IEnumerator PressZCoroutine()
    {
        const float MAX_ALPHA = 0.6f;
        const float MIN_ALPHA = 0.2f;
        const float TIME_TO_FADE = 0.8f;

        TextMeshProUGUI pressZTmp = pressZIndicator.GetComponent<TextMeshProUGUI>();

        while (true)
        {
            Color currentColor = pressZTmp.color;
            currentColor.a = MAX_ALPHA;
            pressZTmp.color = currentColor;

            float elapsed = 0f;
            while (elapsed < TIME_TO_FADE)
            {
                float t = elapsed / TIME_TO_FADE;
                currentColor.a = Mathf.Lerp(MAX_ALPHA, MIN_ALPHA, t);
                pressZTmp.color = currentColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < TIME_TO_FADE)
            {
                float t = elapsed / TIME_TO_FADE;
                currentColor.a = Mathf.Lerp(MIN_ALPHA, MAX_ALPHA, t);
                pressZTmp.color = currentColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            currentColor.a = MIN_ALPHA;
            pressZTmp.color = currentColor;
        }
    }

    public void OnUpdate()
    {
       currentEvent?.UpdateEvent();
    }

    public void StartTestDialogue()
    {
        events.Clear();
        events.Add(new DialogueEvent("Test Dialogue Haha", "Test Speaker", SpeakerSprite.MC));
        events.Add(new DialogueEvent("Woahhhh let me test how loooong lines work here too haha yes.", "Test Haha", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("ok i think it works ending it now", "Done Speaker", SpeakerSprite.MC));
        
        StartNextEvent();
    }

    public void StartNextEvent()
    {
        if (events.Count == 0)
        {
            currentEvent = null;
            GameManager.instance.EndDialogueState();
            return;
        }

        currentEvent = events[0];
        events.RemoveAt(0);
        currentEvent.StartEvent();
    }
}
