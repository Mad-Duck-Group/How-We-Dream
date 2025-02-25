using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine;
using System.Threading.Tasks;

public class AnalyticsInitializer : MonoBehaviour
{
    private async void Awake()
    {
        await InitializeServicesAsync();
    }

    private async Task InitializeServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("✅ Analytics initialized and data collection started.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Analytics initialization failed: {e.Message}");
        }
    }
}