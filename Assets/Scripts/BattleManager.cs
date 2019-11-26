using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get { return m_stn; } }
    static BattleManager m_stn;

    [Tooltip("List of enemies for the party to fight. Battle music and background will be determined by the enemy in slot 0")]
    public TECF_BattleProfile[] enemies;

    [Tooltip("List of party members (maximum 4)")]
    public TECF_BattleProfile[] partyMembers = new TECF_BattleProfile[4];

    [HideInInspector]
    public List<TECF_PartyEntity> partyEntities = new List<TECF_PartyEntity>();

    [HideInInspector]
    public List<TECF_EnemyEntity> enemyEntities = new List<TECF_EnemyEntity>();

    public ePartySlot currPartySlot;
    public eEnemySlot currEnemySelect;

    public float BaseDecayRate = 0.05f;
    public float ActionLineSwitchRate = 0.25f;

    public Color deathTint = Color.red;
    public Color deathHighlight = Color.magenta;

    List<TECF.EntityCommand>    _commandList = new List<TECF.EntityCommand>();
    public TECF.EntityCommand   CurrentCommand;        // The current command being formulated that will be added into the command list

    private void OnEnable()
    {
        EventManager.StartListening("NextPartyMember", OnNextPartyMember);
        EventManager.StartListening("ConfirmEnemySelect", OnConfirmEnemySelect);
        EventManager.StartListening("RegisterAttack", OnRegisterAttack);
        EventManager.StartListening("RunThroughCommands", OnRunThroughCommands);
        EventManager.StartListening("PartyUnconscious", OnPartyUnconscious);
        EventManager.StartListening("TameEnemy", OnTameEnemy);
    }

    private void OnDisable()
    {
        EventManager.StopListening("NextPartyMember", OnNextPartyMember);
        EventManager.StopListening("ConfirmEnemySelect", OnConfirmEnemySelect);
        EventManager.StopListening("RegisterAttack", OnRegisterAttack);
        EventManager.StopListening("RunThroughCommands", OnRunThroughCommands);
        EventManager.StopListening("PartyUnconscious", OnPartyUnconscious);
        EventManager.StopListening("TameEnemy", OnTameEnemy);
    }

    void OnTameEnemy(IEventInfo a_info)
    {
        // Enemy was the last, freeze hp countdown on party members
        if (enemyEntities.Count == 1)
        {
            foreach (var party in partyEntities)
            {
                party.StopAllCoroutines();
            }
        }
    }

    void OnPartyUnconscious(IEventInfo a_info)
    {
        PartyInfo partyInfo = a_info as PartyInfo;

        if (partyInfo != null)
        {
            // Go to next party member if was in active slot
            if (partyInfo.partySlot == currPartySlot)
            {
                EventManager.TriggerEvent("NextPartyMember");
            }

            // Party member has fainted, tint the whole screen and activate death highlights
            var actionUI = ReferenceManager.Instance.actionPanel.GetComponentsInChildren<Image>();

            foreach (var aui in actionUI)
            {
                aui.color = deathTint;
            }

            var dialogUI = ReferenceManager.Instance.dialogPanel.GetComponentsInChildren<Image>();

            foreach (var dui in dialogUI)
            {
                dui.color = deathTint;
            }

            var partyUI = ReferenceManager.Instance.partyPanel.GetComponentsInChildren<Image>();

            foreach (var pui in partyUI)
            {
                if (pui.CompareTag("Highlight"))
                {
                    pui.color = deathHighlight; 
                }
                else
                {
                    pui.color = deathTint;
                }
            }
        }
    }

    /**
     * @brief Run through all enemies and decide what actions to take. (Should only be called after the player has made their turn)
     * */
    public void DecideEnemyTurns()
    {
        for (int i = 0; i < enemyEntities.Count; ++i)
        {
            // Randomly pick a party member to attack
            TECF_PartyEntity targetPlayer = GetPartyEntityBySlot((ePartySlot)Random.Range(0, partyMembers.Length));
            TECF_EnemyEntity senderEnemy = enemyEntities[i];

            _commandList.Add(new TECF.EntityCommand
            {
                cmdType = TECF.eCommandType.ATTACK,
                sender = senderEnemy,
                target = targetPlayer
            });
        }
    }

    TECF_PartyEntity GetPartyEntityBySlot(ePartySlot a_slot)
    {
        foreach (var pe in partyEntities)
        {
            if (pe.partySlot == a_slot)
            {
                return pe;
            }
        }

        return null;
    }

    /**
     * @brief Calculate the turn value with a given equation that takes the speed of the entity into account.
     * */
    float CalcTurnVal(float a_speed)
    {
        // Randomly decide to add or minus the percentage
        int     percentModifier = (Random.Range(0, 2) == 0 ? 1 : -1);
        //int percentModifier = 1;
        float   halfVal = a_speed * 0.5f;

        return a_speed + halfVal * percentModifier; 
    }

    /**
     * @brief Calculate the damage value that should be applied to the defender, also taking into account status effect modifiers.
     * @param a_status is the status effect of the attack.
     * @param a_attacker is the entity doing the damaging.
     * @param a_defender is the entity being damaged.
     * */
    float CalcDmgVal(eStatusEffect a_status, TECF_BattleEntity a_attacker, TECF_BattleEntity a_defender)
    {
        float dmg = 0;

        switch (a_status)
        {
            // Regular bash
            case eStatusEffect.NORMAL:
                //dmg += (2 * a_attacker.BattleProfile.offense - a_defender.BattleProfile.defense);

                dmg += a_attacker.BattleProfile.offense * (100f / (100f + (a_defender.BattleProfile.defense)));

                int percentModifier = (Random.Range(0, 2) == 0 ? 1 : -1);

                dmg += dmg * 0.25f * percentModifier; 
                break;
            default:
                break;
        }

        return dmg;
    }

    void OnRunThroughCommands(IEventInfo a_info)
    {
        //Debug.Log("RUN THROUGH COMMANDS");

        // Sort list by sender turn order (highest to lowest)
        _commandList.Sort((a, b) => -(CalcTurnVal(a.sender.BattleProfile.speed).CompareTo(CalcTurnVal(b.sender.BattleProfile.speed))));

        // Go through command list and execute appropriate actions
        foreach (var cmd in _commandList)
        {
            //Debug.Log(cmd.cmdType + " " + cmd.sender.entityName + " " + cmd.target.entityName + " " + CalcTurnVal(cmd.sender.BattleProfile.speed));
            switch (cmd.cmdType)
            {
                case TECF.eCommandType.ATTACK:
                    /// Get ready function
                    // Ready party member if party is attacking
                    TECF_PartyEntity party      = cmd.sender as TECF_PartyEntity;
                    UnityAction partyReadyFunc  = null;

                    if (party != null)
                    {
                        partyReadyFunc = () =>
                        {
                            EventManager.TriggerEvent("OnPartyReady", new PartyInfo { partySlot = party.partySlot });
                        };
                    }
                    // Enemy is attacking, unready all other party members
                    else
                    {
                        partyReadyFunc = () =>
                        {
                            EventManager.TriggerEvent("PartyUnready", new PartyInfo { partySlot = ePartySlot.NONE });
                        };
                    }

                    /// Attacking dialog
                    DialogManager.Instance.AddToQueue(new DialogInfo
                    {
                        dialogType = TECF.eDialogType.ATTACKING,
                        senderEntity = cmd.sender,
                        targetEntity = cmd.target,
                        endDialogFuncDelay = ActionLineSwitchRate,
                        startDialogFunc = partyReadyFunc
                    });

                    /// 1. Check for miss
                    int missNum     = Random.Range(0, 100);
                    int missChance  = Mathf.RoundToInt((1f / 16f) * 100);

                    if (missNum <= missChance)
                    {
                        DialogManager.Instance.AddToQueue(new DialogInfo
                        {
                            dialogType = TECF.eDialogType.MISS,
                            senderEntity = cmd.sender,
                            targetEntity = cmd.target,
                            endDialogFuncDelay = ActionLineSwitchRate                    
                        });

                        // No need to check other steps
                        break;
                    }

                    /// 2. Check for dodge
                    int dodgeNum    = Random.Range(0, 100);
                    int dodgeChance = Mathf.RoundToInt(((2 * cmd.target.BattleProfile.speed - cmd.sender.BattleProfile.speed) / 500f) * 100);

                    if (dodgeNum <= dodgeChance)
                    {
                        DialogManager.Instance.AddToQueue(new DialogInfo
                        {
                            dialogType = TECF.eDialogType.DODGED,
                            senderEntity = cmd.sender,
                            targetEntity = cmd.target,
                            endDialogFuncDelay = ActionLineSwitchRate
                        });

                        // No need to check other steps
                        break;
                    }

                    /// 3. Confirmed hit, calculate damage
                    int attackDmg = Mathf.RoundToInt(CalcDmgVal(eStatusEffect.NORMAL, cmd.sender, cmd.target));

                    /// 4. Check for critical hit
                    int critNum = Random.Range(0, 100);
                    int firstCritChance = Mathf.RoundToInt((cmd.sender.BattleProfile.guts / 500f) * 100);
                    int secondCritChance = Mathf.RoundToInt((1f / 20f) * 100);
                    int critChance = Mathf.Max(firstCritChance, secondCritChance);

                    if (critNum <= critChance)
                    {
                        DialogManager.Instance.AddToQueue(new DialogInfo
                        {
                            dialogType = TECF.eDialogType.CRITICAL_HIT,
                            senderEntity = cmd.sender,
                            targetEntity = cmd.target
                        });

                        attackDmg *= 4;
                    }

                    /// 5. Send damage to the target after dialog finishes
                    DialogManager.Instance.AddToQueue(new DialogInfo
                    {
                        strData = attackDmg.ToString(),
                        dialogType = TECF.eDialogType.DAMAGED,
                        senderEntity = cmd.sender,
                        targetEntity = cmd.target,
                        startDialogFunc = ()=>
                        {
                            EventManager.TriggerEvent("TakeDamage", new DamageInfo
                            {
                                dmg = attackDmg,
                                senderEntity = cmd.sender,
                                targetEntity = cmd.target,
                                statusEffect = eStatusEffect.NORMAL
                            });
                        },
                        endDialogFuncDelay = ActionLineSwitchRate
                    });

                    break;
                default:
                    break;
            }
        }

        // Gone through all commands, clear them
        _commandList.Clear();

        // Activate action phase dialog
        EventManager.TriggerEvent("RunDialog", new DialogRunInfo {
            onDialogCompleteFunc = ()=> { EventManager.TriggerEvent("EndActionPhase"); }
        });
    }

    void OnRegisterAttack(IEventInfo a_info)
    {
        // Create current command with known parameters so far (who is attacking, what kind of command it is)
        CurrentCommand = new TECF.EntityCommand { sender = GetPartyEntityBySlot(currPartySlot), cmdType = TECF.eCommandType.ATTACK };

        // Go into enemy selecting
        EventManager.TriggerEvent("EnemySelecting");
    }

    void OnConfirmEnemySelect(IEventInfo a_info)
    {
        // Add action against enemy to the command list
        _commandList.Add(CurrentCommand);

        // Selection done, go to next party member
        EventManager.TriggerEvent("NextPartyMember");
    }

    void OnNextPartyMember(IEventInfo a_info)
    {
        // Unready previous party member (if there was one)
        //if (currPartySlot != ePartySlot.NONE) EventManager.TriggerEvent("PartyUnready", new PartyInfo { partySlot = currPartySlot });

        StartCoroutine("GetNextValidSlot");
    }

    IEnumerator GetNextValidSlot()
    {
        int prevSlot = (int)currPartySlot;
        int newSlot  = prevSlot + 1;

        while (newSlot < partyMembers.Length)
        {
            // Suggested member at slot is unconscious, skip to next
            if (GetPartyEntityBySlot((ePartySlot)newSlot).currentStatus == eStatusEffect.UNCONSCIOUS)
            {
                newSlot++;
            }
            // Found valid member
            else
            {
                currPartySlot = (ePartySlot)newSlot;

                // Ready new party member
                EventManager.TriggerEvent("OnPartyReady", new PartyInfo { partySlot = currPartySlot });

                // No need to search for anymore slots
                yield break;
            }

            yield return null;
        }

        // Next slot is outside bounds, assume player turn is over
        currPartySlot = ePartySlot.NONE;

        EventManager.TriggerEvent("EndPlayerTurn");
    }

    public void OnValidate()
    {
        // Force number of party members to max of 4
        if (partyMembers.Length > 4)
        {
            System.Array.Resize(ref partyMembers, 4);
            Debug.LogWarning("Party members cannot exceed max of 4 members!");
        }
    }

    private void Awake()
    {
        // Only one instance allowed, destroy object
        if (m_stn != null && m_stn != this)
        {
            Debug.LogError("REFERENCEMANAGER::Only one instance allowed of this script! Destroying object.");
            Destroy(this.gameObject);
        }
        else
        {
            m_stn = this;
        }
    }

    private void Start()
    {
        currPartySlot = ePartySlot.NONE;

        /// Priority enemy
        TECF_BattleProfile priorityEnemy = enemies[0];  // Enemy in first slot influences visuals and audio of whole battle

        Debug.Assert(priorityEnemy, "BATTLEMANAGER::No enemies set!");

        // Set enemy background
        ReferenceManager.Instance.enemyBG.sprite = priorityEnemy.battleBG;

        // Add enemy audio to main system and secondary systems
        ReferenceManager.Instance.mainAudio.audioFiles.AddRange(priorityEnemy.battleSFX);
        ReferenceManager.Instance.secondaryAudio.audioFiles.AddRange(priorityEnemy.battleSFX);

        // Start battle music, playing intro first if there is one
        AudioClip introClip = ReferenceManager.Instance.mainAudio.HasClip("INTRO");

        if (introClip)
        {
            // Play intro and then play main loop after it
            ReferenceManager.Instance.mainAudio.SetLooping(false);
            ReferenceManager.Instance.mainAudio.PlayClip("{\"alias\":\"INTRO\",\"volume\":1}");

            ReferenceManager.Instance.secondaryAudio.SetLooping(true);
            ReferenceManager.Instance.secondaryAudio.PlayClipWithDelay("{\"alias\":\"LOOP\",\"volume\":1}", introClip.length, true);
        }
        else if (ReferenceManager.Instance.mainAudio.HasClip("LOOP"))
        {
            ReferenceManager.Instance.mainAudio.SetLooping(true);
            ReferenceManager.Instance.mainAudio.PlayClip("{\"alias\":\"LOOP\",\"volume\":1}");
        }

        /// Enemies
        for (int i = 0; i < enemies.Length; ++i)
        {
            GameObject          eObj    = Instantiate(Resources.Load("Enemy") as GameObject);
            TECF_EnemyEntity   eEe     = eObj.GetComponent<TECF_EnemyEntity>();

            // Set to relevant slot
            eEe.enemySlot = (eEnemySlot)i;

            // Set type
            eEe.entityType = eEntityType.ENEMY;

            // Assign battle profile
            eEe.BattleProfile = enemies[i];

            // Keep track of enemy entity
            enemyEntities.Add(eEe);

            eObj.transform.SetParent(ReferenceManager.Instance.enemyPanel.transform);
        }

        /// Party members
        for (int i = 0; i < partyMembers.Length; ++i)
        {
            GameObject pmObj        = Instantiate(Resources.Load("PartyMember") as GameObject);
            TECF_PartyEntity pmPe   = pmObj.GetComponentInChildren<TECF_PartyEntity>();

            // Assign battle profile
            pmPe.BattleProfile = partyMembers[i];

            // Set type
            pmPe.entityType = eEntityType.PARTY;

            // Set to relevant slot
            pmPe.partySlot = (ePartySlot)i;

            // Keep track of party entity
            partyEntities.Add(pmPe);

            pmObj.transform.SetParent(ReferenceManager.Instance.partyPanel.transform);
        }
    }
}
