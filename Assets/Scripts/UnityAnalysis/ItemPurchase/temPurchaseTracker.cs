using UnityEngine;

public class ItemPurchaseTracker : MonoBehaviour
{
    public void PurchaseItem(string itemName, int itemCount, int playerCurrency)
    {
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackItemPurchase(itemName, itemCount);  // ✅ Correct
        }
        else
        {
            Debug.LogError("🚫 AnalyticsManager not found!");
        }
    }
}