using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/RunDialog")]
public class ARunDialog : IAction
{
    public override void Act(StateManager a_controller)
    {
        base.Act(a_controller);

        // Force dialog manager to display party fainted lines
        DialogManager.Instance.PollForFaint();
    }
}
