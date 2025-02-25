using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsInitializer : MonoBehaviour
{
    async void Awake()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log("Analytics Initialized");
    }
}