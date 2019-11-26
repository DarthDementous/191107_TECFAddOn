using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace TECF
{
    public enum eCommandType
    {
        NONE,
        ATTACK
    }

    public enum eDialogType
    {
        NONE = 0,
        GLOBAL = 100,
        INTRO,
        FAINTED,
        GLOBAL_END,
        COMBAT = 200,
        ATTACKING,
        DAMAGED,
        CRITICAL_HIT,
        MISS,
        DODGED,
        COMBAT_END
    }

    public class EntityCommand
    {
        public eCommandType        cmdType;
        public TECF_BattleEntity   sender;
        public TECF_BattleEntity   target;
    }

}

public class DialogInfo : IEventInfo
{
    public void SetDialog(string a_dialog)
    {
        dialog = a_dialog;
    }
    public string GetDialog()
    {
        return dialog;
    }

    public TECF.eDialogType dialogType;
    public TECF_BattleEntity senderEntity;
    public TECF_BattleEntity targetEntity;
    public string strData;
    public UnityAction startDialogFunc;
    public UnityAction endDialogFunc;
    public float endDialogFuncDelay;
    public int queuePos;

    string dialog;
}

public class DialogRunInfo : IEventInfo
{
    public UnityAction onDialogCompleteFunc;
}

public class PartyInfo : IEventInfo
{
    public ePartySlot partySlot;
}

public class EnemyInfo : IEventInfo
{
    public eEnemySlot enemySlot;
}

public class DamageInfo : IEventInfo
{
    public TECF_BattleEntity senderEntity;
    public TECF_BattleEntity targetEntity;
    public int dmg;
    public eStatusEffect statusEffect;
}

public enum eEntityType
{
    NONE,
    ENEMY,
    PARTY
}

public enum eStatusEffect       // TODO: Add explanations for status effects
{
    NORMAL,
    HEAL,
    ASLEEP,
    BLIND,
    BURNING,
    CONFUSED,
    UNCONSCIOUS
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
    public const string attackTxt = " attacks!";
    public const string dmgTxt = " HP of damage to ";
    public const string critTxt = "SMAAAAAAAAAAAAAAASH!";
    public const string missTxt = "Just missed.";
    public const string dodgeTxt = " dodged swiftly!";
    public const string enemyDeathTxt = " became tame!";
    public const string partyDeathTxt = " got hurt and collapsed.";
}
