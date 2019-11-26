using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "FSM/States/ActionPhase")]
public class SActionPhase : IState
{
    public override void Initialise(StateManager a_controller)
    {
        // Show dialog panel
        //ReferenceManager.Instance.dialogPanel.SetActive(true);

        // Hide action panel
        ReferenceManager.Instance.actionPanel.SetActive(false);

        // Identify enemy actions for this turn
        BattleManager.Instance.DecideEnemyTurns();

        // Run through command list
        EventManager.TriggerEvent("RunThroughCommands");
    }

    public override void Shutdown(StateManager a_controller)
    {
        // Hide dialog panel
        //ReferenceManager.Instance.dialogPanel.SetActive(false);
    }
}
