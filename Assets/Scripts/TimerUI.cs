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

        if (remaining < 30)
        {
            clockFill.color = Color.red;
        }
        else if (remaining < 60)
        {
            clockFill.color = Color.yellow;
        }
        else
        {
            clockFill.color = Color.green;
        }
    }
}