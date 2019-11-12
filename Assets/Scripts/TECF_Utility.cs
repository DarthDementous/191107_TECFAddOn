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

public enum ePartySlot
{
    SLOT_1,
    SLOT_2,
    SLOT_3,
    SLOT_4
}

// Static utility helper class
public static class TECF_Utility
{
    /// Constants
    public const string strIntroTxt = "You confront the ";
}
