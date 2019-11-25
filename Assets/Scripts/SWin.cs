using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/Win")]
public class SWin : IState
{
    public override void Initialise(StateManager a_controller)
    {
        base.Initialise(a_controller);

        // Unready all party members
        EventManager.TriggerEvent("PartyUnready", new PartyInfo { partySlot = ePartySlot.NONE });

        ReferenceManager.Instance.winPanel.SetActive(true);
    }
}
