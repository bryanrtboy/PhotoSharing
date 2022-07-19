using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TypedEvent : UnityEvent<object>
{
}

public class EventManager : MonoBehaviour {

    private Dictionary <string, TypedEvent> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init ();
                }
            }

            return eventManager;
        }
    }

    void Init ()
    {
        eventDictionary ??= new Dictionary<string, TypedEvent>();
    }

    public static void StartListening (string eventName, UnityAction<object> listener)
    {
        TypedEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.AddListener (listener);
        }
        else
        {
            thisEvent = new TypedEvent ();
            thisEvent.AddListener (listener);
            instance.eventDictionary.Add (eventName, thisEvent);
        }
    }

    public static void StopListening (string eventName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        TypedEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.RemoveListener (listener);
        }
    }

    public static void TriggerEvent (string eventName, object data)
    {
        TypedEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke (data);
        }
    }
}

[Serializable]
public class Data
{
    public string m_eventKey;
    public float m_floatEventValue;
    public string m_stringEventValue;
    //public PresetCoordinates m_presetCoordinates;
    public Vector3 m_objectCoordinates = Vector3.zero;
    public Vector3 m_onPointerDown = Vector3.zero;
    public bool m_isInRange = false;
    public bool m_testMode = false;

}
