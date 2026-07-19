using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Treasure : MapElement
{
    private bool collected = false;
    private bool canBePickedUp = true;

    public void OnTrigger(Collider2D other)
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
        AudioManager.Instance.PlayTreasurePickup();

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

        if (OtherTimeObject != null)
        {
            OtherTimeObject.transform.SetParent(holdPoint);
            OtherTimeObject.transform.localPosition = Vector3.zero;
            OtherTimeObject.transform.localRotation = Quaternion.identity;

            Collider2D oCol = OtherTimeObject.GetComponent<Collider2D>();
            if (oCol != null)
                oCol.enabled = false;

            Rigidbody2D oRb = OtherTimeObject.GetComponent<Rigidbody2D>();
            if (oRb != null)
            {
                oRb.simulated = false;
            }

            OtherTimeObject.GetComponent<Treasure>().collected = true;
            OtherTimeObject.GetComponent<Treasure>().canBePickedUp = false;
            OtherTimeObject.GetComponent<SpriteRenderer>().enabled = true;
            OtherTimeObject.transform.localScale = transform.localScale / 2;

        }

        return true;
    }

    public void OnDropped()
    {
        if (!collected)
            return;

        AudioManager.Instance.PlayTreasureDrop();
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
            OtherTimeObject.GetComponent<Treasure>().collected = false;
            Collider2D oCol = OtherTimeObject.GetComponent<Collider2D>();
            if (oCol != null)
                oCol.enabled = true;

            Rigidbody2D oRb = OtherTimeObject.GetComponent<Rigidbody2D>();
            if (oRb != null)
            {
                oRb.simulated = true;
            }

            OtherTimeObject.GetComponent<SpriteRenderer>().enabled = false;
            OtherTimeObject.transform.localScale = transform.localScale;
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

    protected override void OnEnable()
    {
        base.OnEnable();
        if (collected)
        {
            transform.localPosition = Vector2.zero;
        }
    }

    public override void HandleTimeSwap(GameManager.Time newTime)
    {
        if (newTime == nativeTime)
        {
            return;
        }

        base.HandleTimeSwap(newTime);
    }
}