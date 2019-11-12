using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/Intro")]
public class SIntro : IState
{
    public override void Initialise(StateManager a_controller)
    {
        // Activate dialog panel
        ReferenceManager.Instance.dialogPanel.SetActive(true);

        // Display intro text
        string enemyTxt = "";

        if (BattleManager.Instance.enemies.Length != 0)     // At least one enemy
        {
            enemyTxt += BattleManager.Instance.enemies[0].entityName;
        }
        if (BattleManager.Instance.enemies.Length > 1 && BattleManager.Instance.enemies.Length < 3) // Two enemies
        {
            enemyTxt += " and its cohort";
        }
        else if (BattleManager.Instance.enemies.Length > 2) // More than two enemies
        {
            enemyTxt += " and its cohorts";
        }

        EventManager.TriggerEvent("OnDisplayDialog", new DialogInfo
        {
            dialog = TECF_Utility.strIntroTxt + enemyTxt,
            endDialogFunc = () => { EventManager.TriggerEvent("EndIntro"); },
            endDialogFuncDelay = 1
        });
    }

    public override void Shutdown(StateManager a_controller)
    {
        // Hide dialog panel
        ReferenceManager.Instance.dialogPanel.SetActive(false);
    }
}
