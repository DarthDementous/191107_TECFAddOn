using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TECF;

[CreateAssetMenu(menuName = "FSM/Actions/RunDialog")]
/**
 * @brief Polls dialog manager for faint dialog so it can be displayed even when not in the action phase.
 * */
public class ARunDialog : IAction
{
    public override void Act(StateManager a_controller)
    {
        base.Act(a_controller);

        // Force dialog manager to display party fainted lines
        DialogManager.Instance.PollForFaint();
    }
}
