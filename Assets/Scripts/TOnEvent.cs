using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Transitions/OnEvent")]
/**
 * @brief Transition based on event being externally called.
 * */
public class TOnEvent : ITransition
{

    bool b_ready;        // Whether ready to transition or not (resets to false before each run-time)

    public bool b_useString;
    public string eventName;

    public bool b_fireEventOnChange;
    public string onChangeEvent;

    private void OnDisable()
    {
        if (b_useString)
        {

            EventManager.StopListening(eventName, SetReady);
        }
    }

    public void SetReady(IEventInfo a_info)
    {
        b_ready = true;
    }

    // Called by Unity event
    public void SetReady()
    {
        b_ready = true;
    }

    public override void Initialise(StateManager a_controller)
    {
        b_ready = false;

        if (b_useString)
        {

            EventManager.StartListening(eventName, SetReady);
        }
    }

    public override bool Decide(StateManager a_controller)
    {
        if (b_ready)
        {
            if (b_fireEventOnChange)
            {
                EventManager.TriggerEvent(onChangeEvent);
            }
            return true;
        }

        return false;
    }
}
