using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "FSM/States/Unconscious")]
public class SUnconscious : IState
{
    public override void Initialise(StateManager a_controller)
    {
        TECF_PartyEntity party = a_controller.GetComponent<TECF_PartyEntity>();
        
        // Visually update screen
        EventManager.TriggerEvent("PartyUnconscious", new PartyInfo { partySlot = party.partySlot});
    }

    public override void Shutdown(StateManager a_controller)
    {

    }
}

