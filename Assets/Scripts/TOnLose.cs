using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TECF;

[CreateAssetMenu(menuName = "FSM/Transitions/OnLose")]
public class TOnLose : ITransition {

    public override bool Decide(StateManager a_controller)
    {
        foreach (var party in BattleManager.Instance.PartyEntities)
        {
            if (party.CurrentStatus != eStatusEffect.UNCONSCIOUS)
            {
                return false;
            }
        }

        return true;
    }

}
