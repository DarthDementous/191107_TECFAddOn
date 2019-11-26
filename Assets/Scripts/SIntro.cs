using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TECF;

[CreateAssetMenu(menuName = "FSM/States/Intro")]
public class SIntro : IState
{
    public override void Initialise(StateManager a_controller)
    {
        // Activate dialog panel
        //ReferenceManager.Instance.dialogPanel.SetActive(true);

        // Display intro text
        string enemyTxt = "";

        if (BattleManager.Instance.Enemies.Length != 0)     // At least one enemy
        {
            enemyTxt += BattleManager.Instance.Enemies[0].entityName;
        }
        if (BattleManager.Instance.Enemies.Length > 1 && BattleManager.Instance.Enemies.Length < 3) // Two enemies
        {
            enemyTxt += " and its cohort";
        }
        else if (BattleManager.Instance.Enemies.Length > 2) // More than two enemies
        {
            enemyTxt += " and its cohorts";
        }

        //// Add test dialog line
        //DialogManager.Instance.DialogQueue.Enqueue(new DialogInfo { dialog = "Behold some test dialog!" });

        // Add intro text dialog
        DialogManager.Instance.AddToQueue(new DialogInfo
        {
            dialogType = TECF.eDialogType.INTRO,
            strData = enemyTxt,
            endDialogFunc = () => { EventManager.TriggerEvent("EndIntro"); },
            endDialogFuncDelay = 1
        });

        // Run through intro dialog
        EventManager.TriggerEvent("RunDialog");
    }

    public override void Shutdown(StateManager a_controller)
    {
        // Hide dialog panel
        //ReferenceManager.Instance.dialogPanel.SetActive(false);
    }
}
