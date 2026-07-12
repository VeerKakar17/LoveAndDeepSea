using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
    private bool collected = false;
    private bool canBePickedUp = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TREASURE");
        if (collected || !canBePickedUp)
            return;

        if (!other.CompareTag("Player"))
            return;


        // Tell the game the player has the treasure
        other.GetComponent<Player>().HoldTreasure(this);

        // Rest of Pickup Logic
        TryPickUp(other.gameObject);
    }

    // For enemies trying to pick this up
    public bool TryPickUp(GameObject other)
    {
        if (!canBePickedUp)
        {
            return false;
        }

        collected = true;

        // Stop physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Attach to player
        Transform holdPoint = other.transform.Find("HoldPoint");
        transform.SetParent(holdPoint);

        // Position in the player's hands
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity;

        MapElement mapElement = GetComponent<MapElement>();

        if (mapElement.OtherTimeObject != null)
        {
            mapElement.OtherTimeObject.transform.SetParent(holdPoint);
            mapElement.OtherTimeObject.transform.localPosition = Vector3.zero;
            mapElement.OtherTimeObject.transform.localRotation = Quaternion.identity;
        }

        return true;
    }

    public void OnDropped()
    {
        if (!collected)
            return;

        Debug.Log("Dropping: " + gameObject.name);
        Debug.Log("Parent before: " + transform.parent);
        canBePickedUp = false;
        StartCoroutine(PickupCooldown());

        collected = false;

        transform.SetParent(null);
        Debug.Log("Parent after: " + transform.parent);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        transform.position += Vector3.down * 0.5f;
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.down * 2f, ForceMode2D.Impulse); // drop effect
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        Debug.Log("Treasure dropped!");

        MapElement mapElement = GetComponent<MapElement>();

        if (mapElement.OtherTimeObject != null)
        {
            mapElement.OtherTimeObject.transform.SetParent(null);
        }
        
    }

    private IEnumerator PickupCooldown()
    {
        yield return new WaitForSeconds(.5f);
        canBePickedUp = true;
    }

    private void Update()
    {
        // clamp pos
        
    }

    private void OnEnable()
    {
        if (collected)
        {
            transform.localPosition = Vector2.zero;
        }
    }
}