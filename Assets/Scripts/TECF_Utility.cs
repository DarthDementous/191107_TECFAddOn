using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

public class DialogInfo : IEventInfo
{
    public string dialog;
    public UnityAction endDialogFunc;
    public float endDialogFuncDelay;
}

public class PartyInfo : IEventInfo
{
    public ePartySlot partySlot;
}

public class EnemyInfo : IEventInfo
{
    public eEnemySlot enemySlot;
}

public enum ePartySlot
{
    NONE = -1,
    SLOT_1,
    SLOT_2,
    SLOT_3,
    SLOT_4
}

public enum eEnemySlot
{
    A,
    B,
    C,
    D,
    E,
    F
}

// Static utility helper class
public static class TECF_Utility
{
    /// Constants
    public const string strIntroTxt = "You confront the ";
}
