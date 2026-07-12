using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] public Vector2 minBounds;
    [SerializeField] public Vector2 maxBounds;

    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 velocity;

    private Camera cam;

    public bool enableBounds = true;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (enableBounds) {
        Vector3 target = player.position;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        target.x = Mathf.Clamp(
            target.x,
            minBounds.x + halfWidth,
            maxBounds.x - halfWidth
        );

        target.y = Mathf.Clamp(
            target.y,
            minBounds.y + halfHeight,
            maxBounds.y - halfHeight
        );

        target.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            target,
            ref velocity,
            smoothTime
        );
        }
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
