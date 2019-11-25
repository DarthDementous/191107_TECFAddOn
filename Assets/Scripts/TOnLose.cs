using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(menuName = "FSM/Transitions/OnLose")]
public class TOnLose : ITransition {

    public override bool Decide(StateManager a_controller)
    {
        foreach (var party in BattleManager.Instance.partyEntities)
        {
            if (party.currentStatus != eStatusEffect.UNCONSCIOUS)
            {
                return false;
            }
        }

        return true;
    }

}
