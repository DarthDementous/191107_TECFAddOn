using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IEventInfo  {

    public GameObject sender;      // Who invoked the event
    public GameObject target;      // Who the event was targeted at
}

public class GameplayEvent : UnityEvent<IEventInfo> { }

/**
 * @brief Singleton class that holds onto generic gameplay events that classes can subscribe to or invoke.
 * */
public class EventManager : MonoBehaviour {

    Dictionary<string, GameplayEvent> eventDict;       // Allow events to be added and removed dynamically

    static EventManager eventManager;

    public static EventManager instance {
        get {
            // Assign static reference
            if (!eventManager) {
                eventManager = Object.FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager) { Debug.LogError("ERROR::EVENT_MANAGER::There needs to be an EventManager script on a game object.");}
                
                // Initialise event manager
                else {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    /**
     * @brief Called upon first reference assignment to prepare the event manager.
     * */
    private void Init() {
        // Initialise event dictionary
        if (eventDict == null) {
            eventDict = new Dictionary<string, GameplayEvent>();
        }
    }

    /**
     * @brief Subscribe function to a specified event.
     * @param a_eventName is the name of the event.
     * @param a_listener is the function to subscribe to the event with.
     * */
    public static void StartListening(string a_eventName, UnityAction<IEventInfo> a_listener) {

        GameplayEvent foundEvent = null;

        // Event already exists
        if (instance.eventDict.TryGetValue(a_eventName, out foundEvent)) {

            foundEvent.AddListener(a_listener);
        }
        // Event doesn't exist, create new event and add listener
        else {
            foundEvent = new GameplayEvent();

            foundEvent.AddListener(a_listener);
            instance.eventDict.Add(a_eventName, foundEvent);
        }
    }

    /**
     * @brief Unsubscribe function from a specified event.
     * @param a_eventName is the name of the event.
     * @param a_listener is the function to subscribe to the event with.
     * */
    public static void StopListening(string a_eventName, UnityAction<IEventInfo> a_listener) {

        if (eventManager == null) return;   // Account for event manager being destroyed first in a clean-up situation

        GameplayEvent foundEvent = null;

        // Event exists
        if (instance.eventDict.TryGetValue(a_eventName, out foundEvent)) {

            foundEvent.RemoveListener(a_listener);
        }
    }

    /**
     * @brief Invoke a specified event.
     * @param a_eventName is the name of the event.
     * @param a_eventInfo is the information to pass into the event.
     * */
    public static void TriggerEvent(string a_eventName, IEventInfo a_eventInfo = null) {

        GameplayEvent foundEvent = null;

        // Event exists
        if (instance.eventDict.TryGetValue(a_eventName, out foundEvent)) {

            foundEvent.Invoke(a_eventInfo);
        }
    }
}
