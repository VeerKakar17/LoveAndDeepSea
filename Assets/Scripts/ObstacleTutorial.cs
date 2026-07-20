using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ObstacleTutorial : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;

    public string[] tutorialPages;

    private bool tutorialShown = false;
    private bool tutorialActive = false;
    private int currentPage = 0;

    void Start() {
        tutorialPages = new string[2];

        tutorialPages[0] = "Press F on your keyboard to use the clock artifact to switch between modern day and ancient times! You can use this to bypass obstaces. However, the ancient fish can steal your treasure or stun you for a few moments.";
        tutorialPages[1] = "Keep an eye on the clock in the bottom left corner. This shows you how much time you have left in the ancient world. You must obtain the treasure before time runs out. Good luck traveler!";

    }
    
    void Update()
    {
        if (tutorialActive && Keyboard.current.zKey.wasPressedThisFrame)
        {
            currentPage++;

            if (currentPage >= tutorialPages.Length)
            {
                CloseTutorial();
            }
            else
            {
                tutorialText.text = tutorialPages[currentPage];
            }
        }
    }

    public void ShowTutorial()
    {
        if (tutorialShown)
            return;

        tutorialShown = true;
        tutorialActive = true;
        currentPage = 0;

        tutorialPanel.SetActive(true);
        tutorialText.text = tutorialPages[currentPage];

        Time.timeScale = 0f;
    }

    void CloseTutorial()
    {
        tutorialActive = false;
        tutorialPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowTutorial();
        }
    }
}