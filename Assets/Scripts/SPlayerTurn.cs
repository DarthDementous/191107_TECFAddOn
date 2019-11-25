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

        //// Decide which party member goes first
        //EventManager.TriggerEvent("NextPartyMember");
    }

    public override void Shutdown(StateManager a_controller)
    {
        // Hide player turn GUI
        ReferenceManager.Instance.actionPanel.SetActive(false);
    }
}
