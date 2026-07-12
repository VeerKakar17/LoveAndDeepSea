using UnityEngine;
using System.Collections;

public class TimeTransitionEffect : MonoBehaviour
{
    public Material material;
    public Transform player;

    public float duration = 1f;

    bool swapping;


    public IEnumerator Play()
    {
        swapping = true;


        Vector3 screenPos =
            UnityEngine.Camera.main.WorldToViewportPoint(
                player.position
            );


        material.SetVector(
            "_Center",
            screenPos
        );


        float radius = 0;


        while(radius < 1.5f)
        {
            radius +=
                Time.deltaTime *
                1.5f;


            material.SetFloat(
                "_Radius",
                radius
            );


            yield return null;
        }


        swapping = false;
    }


    private void OnRenderImage(
        RenderTexture source,
        RenderTexture destination)
    {
        Graphics.Blit(
            source,
            destination,
            material
        );
    }
}