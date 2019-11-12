using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Transitions/OnKeyPress")]
public class TOnKeyPress : ITransition
{
    public KeyCode key;

    public override bool Decide(StateManager a_controller)
    {
        if (Input.GetKeyDown(key))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
