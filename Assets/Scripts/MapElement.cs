using UnityEngine;
using System.Text.RegularExpressions;

public class MapElement : MonoBehaviour
{
    public GameManager.Time nativeTime { get; set; } = GameManager.Time.ModernTime;
    public MapElement OtherTimeObject { get; set; } = null;
    private Vector3 OTHER_POSITION_OFFSET = new Vector3(0, 10000f, 0);

    void Start()
    {
        if (nativeTime == GameManager.Time.AncientTime)
        {
            return;
        }

        // Get name without the (x) at the end if it's a duplicate
        string objBaseName = Regex.Replace(gameObject.name, @" \(\d+\)$", "");

        // Instantiate an ancient copy from the prefab
        GameObject ancientPrefab = Resources.Load<GameObject>("Prefabs/Ancient/" + objBaseName + "_ancient");

        if (ancientPrefab != null)
        {
            GameObject ancientObject = Instantiate(ancientPrefab, transform.position + OTHER_POSITION_OFFSET, transform.rotation);
            ancientObject.SetActive(false);
            OtherTimeObject = ancientObject.GetComponent<MapElement>();
            OtherTimeObject.nativeTime = GameManager.Time.AncientTime;
            OtherTimeObject.OtherTimeObject = this;
        }
    }
   
    // Handler called when swapping off of this element's native time to other time
    public void HandleTimeSwap(GameManager.Time newTime)
    {
        // Edge case should not be possible, but adding just in case
        if (newTime == nativeTime)
        {
            return;
        }

        // Sync position
        if (nativeTime == GameManager.Time.ModernTime)
        {
            OtherTimeObject.gameObject.transform.position = transform.position + OTHER_POSITION_OFFSET;
        } else
        {
            OtherTimeObject.gameObject.transform.position = transform.position - OTHER_POSITION_OFFSET;
        }

        OtherTimeObject.gameObject.SetActive(true); // Enable other object
        gameObject.SetActive(false); // Disable self
    }

    private void OnEnable()
    {
        GameManager.OnTimeSwap += HandleTimeSwap;
    }

    private void OnDisable()
    {
        GameManager.OnTimeSwap -= HandleTimeSwap;
    }
}
