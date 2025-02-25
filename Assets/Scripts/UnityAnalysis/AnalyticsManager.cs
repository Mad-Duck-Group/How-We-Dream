using Unity.Services.Analytics;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TrackItemPurchase(string itemName, int itemCount)
    {
        var customEvent = new CustomEvent("item_purchase")
        {
            { "itemName", itemName },
            { "itemCount", itemCount }
        };

        AnalyticsService.Instance.RecordEvent(customEvent);  // ✅ Correct usage
        Debug.Log($"Item purchase event sent: {itemName} x{itemCount}");
    }
}