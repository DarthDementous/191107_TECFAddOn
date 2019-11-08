using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class TECF_BattleEntity : MonoBehaviour
{
    [Tooltip("NONE=Default, should never be set to this." +
        "\nENEMY=The player party will fight against them" +
        "PARTY=The player party will fight with them")]
    eEntityType     entityType;
    
    [HideInInspector]
    StateManager m_stateManager;    // TODO: Get reference to state manager on object

    [Tooltip("Current affliction on the entity, will affect their behaviour if it isn't NORMAL")]
    eStatusEffect currentStatus;

    [Tooltip("Battle information to use for this entity. Determines their stats, abilities, audio, and visuals.")]
    TECF_BattleProfile battleProfile;

    [HideInInspector]
    bool b_autoFight;   // Whether AI should take over for this entity or be player controlled
}
