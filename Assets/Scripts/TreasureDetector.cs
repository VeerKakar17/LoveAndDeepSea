using UnityEngine;

public class TreasureDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.transform.parent.GetComponent<Treasure>().OnTrigger(collision);
            return;
        }
    }
}
