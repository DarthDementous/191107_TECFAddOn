using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct KeyEventObj
{
    public KeyCode key;
    public string eventName;
}

[CreateAssetMenu(menuName = "FSM/Transitions/OnKeyPress")]
public class TOnKeyPress : ITransition
{
    public KeyEventObj[] keyEvents;

    public override bool Decide(StateManager a_controller)
    {
        foreach (var ke in keyEvents)
        {
            if (Input.GetKeyDown(ke.key))
            {
                // Fire event if specified
                if (ke.eventName != "") EventManager.TriggerEvent(ke.eventName);

                return true;
            }
        }

        return false;
    }
}
