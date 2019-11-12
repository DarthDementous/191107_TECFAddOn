using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/PlayerTurn")]
public class SPlayerTurn : IState
{
    public override void Initialise(StateManager a_controller)
    {
        // Show player turn GUI
        ReferenceManager.Instance.actionPanel.SetActive(true);

        // Move first party member into ready position
        EventManager.TriggerEvent("OnPartyReady", new PartyInfo { partySlot = ePartySlot.SLOT_1 });
    }

    public override void Shutdown(StateManager a_controller)
    {
        // Hide player turn GUI
        ReferenceManager.Instance.actionPanel.SetActive(false);
    }
}
