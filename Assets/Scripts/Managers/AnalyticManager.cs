using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.Serialization;

public class AnalyticManager : MonoSingleton<AnalyticManager>
{
    #region Data Structures

    private struct EventData
    {
        public string eventName;
        public List<EventParameterData> eventParameters;
    }

    private struct EventParameterData
    {
        public string parameterName;
        public object parameterValue;
    }

    [Serializable]
    private struct EventNameData
    {
        public string eventName;
        public List<string> parameterNames;
    }

    private enum EventType
    {
        OnLevelComplete,
        OrderCompletionTime
    }

    #endregion

    [SerializeField, SerializedDictionary("Event Type", "Event Name")]
    private SerializedDictionary<EventType, EventNameData> eventNameDictionary;

    void Start()
    {
        Initialize();
    }

    private async void Initialize()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }

    public void OnLevelCompleted(int minigameFailedCount, int orderCompletionCount)
    {
        var arrayList = new ArrayList
        {
            minigameFailedCount,
            orderCompletionCount
        };
        var eventData = CreateEventData(EventType.OnLevelComplete, arrayList);
        SendEvent(eventData);
    }

    public void OnOrderCompletionTime(float time)
    {
        var arrayList = new ArrayList { time };
        var eventData = CreateEventData(EventType.OrderCompletionTime, arrayList);
        SendEvent(eventData);
    }

    private EventData CreateEventData(EventType eventType, ArrayList parameterValues)
    {
        string eventName = eventNameDictionary[eventType].eventName;
        List<string> parameterNames = eventNameDictionary[eventType].parameterNames;
        List<EventParameterData> eventParameters = new List<EventParameterData>();
        
        for (int i = 0; i < parameterValues.Count; i++)
        {
            eventParameters.Add(new EventParameterData
            {
                parameterName = parameterNames[i],
                parameterValue = parameterValues[i]
            });
        }
        
        return new EventData
        {
            eventName = eventName,
            eventParameters = eventParameters
        };
    }

    private void SendEvent(EventData eventData)
    {
        CustomEvent customEvent = new CustomEvent(eventData.eventName);

        void SendEvent(EventData eventData)
        {
            CustomEvent customEvent = new CustomEvent(eventData.eventName);
            eventData.eventParameters.ForEach(parameter =>
            {
                customEvent.Add(parameter.parameterName, parameter.parameterValue);
            });
            AnalyticsService.Instance.RecordEvent(customEvent);
            Debug.Log($"Event sent: {eventData.eventName}");
        }
    }
}
