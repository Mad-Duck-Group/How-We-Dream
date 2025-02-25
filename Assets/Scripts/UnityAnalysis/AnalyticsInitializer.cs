using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Analytics Initialized");

            GiveConsent(); // Start collecting data with consent
        }
        catch (System.Exception e) // ✅ Changed here
        {
            Debug.LogError($"Analytics initialization failed: {e.Message}"); // ✅ Use e.Message for error details
        }
    }

    void GiveConsent()
    {
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log("User consent given. Analytics data collection started.");
    }
}