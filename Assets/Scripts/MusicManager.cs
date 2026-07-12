using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [SerializeField] public AudioSource modernSource;
    [SerializeField] public AudioSource ancientSource;
    [SerializeField] public float fadeTime = 0.5f;

    Coroutine fadeRoutine;

    void Start()
    {
        modernSource.Play();
        ancientSource.Play();

        modernSource.volume = 1f;
        ancientSource.volume = 0f;
    }

    private void OnEnable()
    {
        GameManager.OnTimeSwap += HandleSwap;
    }

    private void OnDisable()
    {
        GameManager.OnTimeSwap -= HandleSwap;
    }

    void HandleSwap(GameManager.Time time)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeMusic(
            time == GameManager.Time.AncientTime
        ));
    }

    IEnumerator FadeMusic(bool ancient)
    {
        float startModern = modernSource.volume;
        float startAncient = ancientSource.volume;

        float targetModern = ancient ? 0f : 1f;
        float targetAncient = ancient ? 1f : 0f;

        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            float a = t / fadeTime;

            modernSource.volume = Mathf.Lerp(
                startModern,
                targetModern,
                a
            );

            ancientSource.volume = Mathf.Lerp(
                startAncient,
                targetAncient,
                a
            );

            yield return null;
        }

        modernSource.volume = targetModern;
        ancientSource.volume = targetAncient;
    }
}