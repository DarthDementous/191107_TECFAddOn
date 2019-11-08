using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Tooltip("List of enemies for the party to fight. Battle music and background will be determined by the enemy in slot 0")]
    TECF_BattleEntity[] enemies;

    [Tooltip("List of party members (maximum 4)")]
    TECF_BattleEntity[] partyMembers = new TECF_BattleEntity[4];
}
