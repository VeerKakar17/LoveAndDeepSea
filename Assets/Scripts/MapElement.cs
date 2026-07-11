using UnityEngine;
using System.Text.RegularExpressions;

public class MapElement : MonoBehaviour
{
    [SerializeField] private GameObject ancientPrefab;
    public GameManager.Time nativeTime { get; set; } = GameManager.Time.ModernTime;
    public MapElement OtherTimeObject { get; set; } = null;
    private Vector3 OTHER_POSITION_OFFSET = new Vector3(0, GameManager.TIME_Y_OFFSET, 0);

    void Start()
    {
        if (nativeTime == GameManager.Time.AncientTime)
        {
            return;
        }

        if (ancientPrefab == null)
        {
            ancientPrefab = gameObject;
        }

        GameObject ancientObject = Instantiate(ancientPrefab, transform.position + OTHER_POSITION_OFFSET, transform.rotation);
        ancientObject.SetActive(false);
        OtherTimeObject = ancientObject.GetComponent<MapElement>();
        OtherTimeObject.nativeTime = GameManager.Time.AncientTime;
        OtherTimeObject.OtherTimeObject = this;
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
