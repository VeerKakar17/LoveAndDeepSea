using UnityEngine;
using System.Collections;

public class TimelineCamera : MonoBehaviour
{
    public Transform player;
    public bool ancient;

    void LateUpdate()
    {
        Vector3 pos = player.position;

        if (ancient)
            pos.y += GameManager.TIME_Y_OFFSET;

        pos.z = transform.position.z;

        transform.position = pos;
    }
}