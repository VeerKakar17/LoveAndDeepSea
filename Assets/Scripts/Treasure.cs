using UnityEngine;

using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TREASURE");
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        collected = true;

        // Tell the game the player has the treasure
        // PlayerInventory.instance.HasTreasure = true;

        // Stop physics
        // Rigidbody rb = GetComponent<Rigidbody>();
        // if (rb != null)
        // {
        //     rb.isKinematic = true;
        //     rb.linearVelocity = Vector3.zero;
        // }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Attach to player
        Transform holdPoint = other.transform.Find("HoldPoint");
        transform.SetParent(holdPoint);

        // Position in the player's hands
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        MapElement mapElement = GetComponent<MapElement>();

        if (mapElement.OtherTimeObject != null)
        {
            mapElement.OtherTimeObject.transform.SetParent(holdPoint);
            mapElement.OtherTimeObject.transform.localPosition = Vector3.zero;
            mapElement.OtherTimeObject.transform.localRotation = Quaternion.identity;
        }
    }
}