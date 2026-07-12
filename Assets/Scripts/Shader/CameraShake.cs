using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Shake(float amount, float duration)
    {
        Vector3 start = transform.localPosition;

        float t = 0;

        while(t < duration)
        {
            transform.localPosition =
                start + (Vector3)(Random.insideUnitCircle * amount);

            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = start;
    }
}
