using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityCommunity.UnitySingleton;
using UnityEngine;

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
        public string parameterName;
    }

    private enum EventType
    {
        OrderCompleted,
        MinigameFailed,
        OrderCompletionTime,
        OnOrderFailed,
        OnOrderPartial,
        OrderCancled
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

    public void OnOrderCompleted()
    {
        var eventData = CreateEventData(EventType.OrderCompleted, 1);
        SendEvent(eventData);
    }

    public void OnMinigameFailed()
    {
        var eventData = CreateEventData(EventType.MinigameFailed, 1);
        SendEvent(eventData);
    }

    public void OnOrderCompletionTime(float time)
    {
        var eventData = CreateEventData(EventType.OrderCompletionTime, time);
        SendEvent(eventData);
    }

    public void OnOrderFailed()
    {
        var eventData = CreateEventData(EventType.OnOrderFailed, 1);
        SendEvent(eventData);
    }

    public void OnOrderPartial()
    {
        var eventData = CreateEventData(EventType.OnOrderPartial, 1);
        SendEvent(eventData);
    }

    public void OrderCancled()
    {
        var eventData = CreateEventData(EventType.OrderCancled, 1);
        SendEvent(eventData);
    }

    private EventData CreateEventData(EventType eventType, object parameterValue)
    {
        string eventName = eventNameDictionary[eventType].eventName;
        string parameterName = eventNameDictionary[eventType].parameterName;
        List<EventParameterData> eventParameters = new List<EventParameterData>
        {
            new()
            {
                parameterName = parameterName,
                parameterValue = parameterValue
            }
        };
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
