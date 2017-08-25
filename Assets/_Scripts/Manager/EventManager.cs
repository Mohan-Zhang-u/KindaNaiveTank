using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : PersistentSingleton<EventManager>
{

    private Dictionary<string, UnityEvent> eventDictionary;

    private static EventManager _EventManager;

    // so, no need for s_instance.
    public static EventManager instance
    {
        get
        {
            if (!_EventManager)
            {
                _EventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!_EventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    _EventManager.Init();
                }
            }

            return _EventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            Debug.Log("<color=red>Notice!Listen to event that doesn't exist!</color>");
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (_EventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
        else
        {
            Debug.Log("<color=red>Notice!Stoplisten to event that doesn't exist!</color>");
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
