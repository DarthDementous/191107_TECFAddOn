using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TECF;

[CreateAssetMenu(menuName = "FSM/States/GameOver")]
public class SGameOver : IState
{
    public override void Initialise(StateManager a_controller)
    {
        base.Initialise(a_controller);

        // Unready all party members
        EventManager.TriggerEvent("PartyUnready", new PartyInfo { partySlot = ePartySlot.NONE });

        ReferenceManager.Instance.gameOverPanel.SetActive(true);
    }
}