using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/EnemySelect")]
public class SEnemySelect : IState
{
    public override void Initialise(StateManager a_controller)
    {
        // Show enemy select GUI
        ReferenceManager.Instance.actionPanel.SetActive(true);
        ReferenceManager.Instance.enemySelectPanel.SetActive(true);
    }

    public override void Shutdown(StateManager a_controller)
    {
        // Hide enemy select GUI
        ReferenceManager.Instance.enemySelectPanel.SetActive(false);

        // Deactivate selecting state on enemies
        EventManager.TriggerEvent("EnemyUnselect");
    }
}
