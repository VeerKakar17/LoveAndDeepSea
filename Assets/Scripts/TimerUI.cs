using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour

{
    [SerializeField] private Image clockFill;

    private float maxTime;

    

    private void Start()
    {
        maxTime = GameManager.instance.RemainingAncientTime;

        GameManager.OnAncientTimeChanged += UpdateClock;
    }

    private void OnDestroy()
    {
        GameManager.OnAncientTimeChanged -= UpdateClock;
    }

    private void UpdateClock(float remaining)
    {
        clockFill.fillAmount = remaining / GameManager.instance.maxAncientTime;

        if (remaining < 15)
        {
            clockFill.color = new Color(1f, 0f, 0f, .5f);
        }
        else if (remaining < 30)
        {
            clockFill.color = new Color(1f, 1f, 0f, .5f);
        }
        else
        {
            clockFill.color = new Color(0f, 1f, 0f, .5f);
        }
    }
}