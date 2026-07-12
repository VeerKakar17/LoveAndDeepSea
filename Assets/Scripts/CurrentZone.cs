using UnityEngine;

public class CurrentZone : MonoBehaviour
{
    [Header("Current Settings")]
    [SerializeField] private float strength = 2f;

    [Tooltip("Degrees. 0 = right, 90 = up")]
    [SerializeField] private float baseAngle = 90f;

    [Header("Direction Variation")]
    [SerializeField] private bool useSineWave = true;
    [SerializeField] private float angleVariation = 30f;
    [SerializeField] private float waveSpeed = 2f;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (!(other.CompareTag("Player") || other.CompareTag("Treasure"))){
            return;
        }

        if(other.CompareTag("Player")){
            Player player = other.GetComponent<Player>();

            float angle = baseAngle;
            float noise = Mathf.PerlinNoise(Time.time, 0) - 0.5f;
            angle += noise * angleVariation;

            if (useSineWave)
            {
                angle += Mathf.Sin(Time.time * waveSpeed) * angleVariation;
            } 

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            if (player != null)
            {
                player.externalForce += direction * strength;
            }
        }

        if(other.CompareTag("Treasure")){
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb == null)
                return;

            float angle = baseAngle;
            float noise = Mathf.PerlinNoise(Time.time, 0) - 0.5f;
            angle += noise * angleVariation;

            if (useSineWave)
            {
                angle += Mathf.Sin(Time.time * waveSpeed) * angleVariation;
            } 

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );


            rb.AddForce(direction * strength);
        }
    }
}
