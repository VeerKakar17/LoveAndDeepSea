using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
// using static UnityEditor.Rendering.MaterialUpgrader;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private GameObject pressZIndicator;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject speakerBox;
    [SerializeField] private Image speakerImage;
    [SerializeField] private List<Sprite> speakerSprites;
    private Coroutine pressZCoroutine = null;

    List<DialogueEvent> events;
    DialogueEvent currentEvent;

    public enum SpeakerSprite
    {
        MC = 0,
        DragonKing = 1,
        None = 2,
    }

    private void Awake()
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
        if (speaker == null)
        {
            speakerText.text = "";
            speakerBox.SetActive(false);
        } else
        {
            speakerText.text = speaker;
            speakerBox.SetActive(true);
        }
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
        speakerBox.SetActive(false);
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

    public void StartIntroDialogue()
    {
        events.Clear();
        events.Add(new DialogueEvent("After yet another long day at your monotonous office job, you find yourself where you end up every day after work:\nthe nearby beach", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("You’ve always been obsessed with legends of dragon kings and long-dead creatures, of a grand palace beneath the primordial sea.\r\n", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("Sigh. If only they were real…My life would be so much more interesting.", "You", SpeakerSprite.MC));
        events.Add(new DialogueEvent("Suddenly, a glint in the sand catches your eye. You reach down to pick up a mysterious, watch-like object brought in by the waves.\r\n", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("Human…I heed your devotion. I shall indulge your wishes.\r\n", "???", SpeakerSprite.None));
        events.Add(new DialogueEvent("Who…who are you?!", "You", SpeakerSprite.MC));
        events.Add(new DialogueEvent("I am the Dragon King, ruler of these seas. You are the only human who still thinks of me as such now, and visits my domain to pay tribute. Your piety shall be rewarded.", "??? (Dragon King)", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("You tire of your current life, do you not?", "Dragon King", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("You nod vigorously, still in awe.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("Good. The artifact you hold will take you between my time and yours. You shall put it to good use.", "Dragon King", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("Human, if you wish for a life with me instead - accept my trial: bring me a treasure from my realms by sunset. If you succeed, you shall have my hand in marriage.", "Dragon King", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("However…if you fail…Did you know, certain creatures eat their own lovers, moments after their passionate embrace? Love is fickle, as unpredictable as the seas.", "Dragon King", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("Do not disappoint me.", "Dragon King (smiling)", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("You barely pause at the warning bells ringing and this blatant red flag waving in front of you before accepting. After all, this red flag was the Dragon King - who was not only real, but an absolute, beautiful, hunk of a man.\r\n", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("Go, follow the currents, return by sunset to my palace. Whether you fail or succeed…I shall be waiting.\r\n", "Dragon King", SpeakerSprite.DragonKing));

        StartNextEvent();
    }

    public void StartGoodEndingDialogue()
    {
        events.Clear();
        events.Add(new DialogueEvent("In a burst of seafoam, the currents bring you to the Dragon King’s palace. It is every bit as brilliant and grand as the legends described, a beacon in the murky depths.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("You walk up the dais and sink into a low bow for the king - no, your king.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("Human, you’ve returned. Have you brought me what I requested of you?", "Dragon King", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("You hold out your hands, the beautiful treasure glittering within your palms.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("The Dragon King stands slowly, and then he smiles.\r\n", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("Human…No, my dear, you have succeeded. Brilliantly. I have no doubts whatsoever about your loyalty to me and our subjects.", "Dragon King", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("He sweeps down from the throne to take the treasure from your hands and raise you from the bow.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("There is no need for that anymore. Come, celebrations are in order! I shall announce to the world that I have finally found a spouse worthy of my love.", "Dragon King (still smiling)", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("Come, let us live the life you’ve always wanted, together!", "Dragon King", SpeakerSprite.DragonKing));

        StartNextEvent();
    }

    public void StartBadEnding()
    {
        events.Clear();

        events.Add(new DialogueEvent("In a burst of seafoam, the currents bring you to the Dragon King’s palace. It is every bit as brilliant and grand as the legends described, a beacon in the murky depths, but you are filled with cold dread.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("You walk up the dais and sink into a low bow for the Dragon King.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("Human, you’ve returned. Have you brought me what I requested of you?", "Dragon King", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("You don’t speak, eyes downcast in shame, holding out trembling hands which are empty.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("The Dragon King’s gaze grows dark and he stands slowly.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("I…see. It seems I was mistaken by the extent of your loyalty. It seems you are not too different from the other humans, as I had otherwise thought.", "Dragon King", SpeakerSprite.DragonKing));
        events.Add(new DialogueEvent("The Dragon King sighs deeply.", null, SpeakerSprite.None));
        events.Add(new DialogueEvent("I had no desire to do this, but promises must be kept. Both yours and mine, and I warned you what would happen if you returned empty handed.\r\n", "Dragon King", SpeakerSprite.DragonKing));

        StartNextEvent();
    }
}
