using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TECF;

[CreateAssetMenu(menuName = "FSM/States/EnemySelect")]
public class SEnemySelect : IState
{
    public override void Initialise(StateManager a_controller)
    {
        // Show enemy select GUI
        ReferenceManager.Instance.actionPanel.SetActive(true);
        ReferenceManager.Instance.enemySelectPanel.SetActive(true);

        // Temporarily disable all action buttons
        var buttons = ReferenceManager.Instance.actionPanel.GetComponentsInChildren<Button>();

        foreach (var b in buttons)
        {
            b.interactable = false;
        }

        // Activate enemy selections
        foreach (var e in BattleManager.Instance.EnemyEntities)
        {
            e.anim.SetBool("Selecting", true);
        }

        // Select first enemy by default
        EventManager.TriggerEvent("SelectEnemy", new EnemyInfo { enemy = BattleManager.Instance.EnemyEntities[0] });
    }

    public override void Shutdown(StateManager a_controller)
    {
        // Hide enemy select GUI
        ReferenceManager.Instance.enemySelectPanel.SetActive(false);

        // Re-enable all action buttons
        var buttons = ReferenceManager.Instance.actionPanel.GetComponentsInChildren<Button>();

        foreach (var b in buttons)
        {
            b.interactable = true;
        }

        // Deactivate enemy selections
        foreach (var e in BattleManager.Instance.EnemyEntities)
        {
            e.anim.SetBool("Selecting", false);
        }
    }
}
