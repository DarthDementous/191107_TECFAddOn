using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TECF
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get { return _stn; } }
        static BattleManager _stn;

        private void Awake()
        {
            // Only one instance allowed, destroy object
            if (_stn != null && _stn != this)
            {
                Debug.LogError("REFERENCEMANAGER::Only one instance allowed of this script! Destroying object.");
                Destroy(this.gameObject);
            }
            else
            {
                _stn = this;
            }
        }

        #region Public Facing Variables
        [Header("Blueprints")]

        [Tooltip("Which prefab to use for enemies")]
        public GameObject EnemyBlueprint;
        [Tooltip("Which prefab to use for party members")]
        public GameObject PartyBlueprint;

        [Header("Combat Encounter Settings")]
        [TextArea]
        [SerializeField]
        private string cesHint = "Hint: Drag in battle profile assets to set up your combat encounter";

        [Tooltip("List of enemies for the party to fight. Battle music and background will be determined by the first enemy")]
        public BattleProfile[] Enemies;
        [Tooltip("List of party members (maximum 4)")]
        public BattleProfile[] PartyMembers = new BattleProfile[4];

        [Header("Combat Settings")]

        [Tooltip("How much to times an attack by if its a critical hit")]
        public int CriticalHitModifier = 4;
        [Tooltip("Percentage chance that an attack will miss")]
        [Range(0f, 1f)]
        public float ChanceToMiss = 0.0625f;

        [Header("Interface Settings")]

        [Tooltip("What color to tint all the UI to when a party member is unconscious")]
        public Color DeathTint = Color.red;
        [Tooltip("What color to change the party name bar to when a party member is unconscious")]
        public Color DeathHighlight = Color.magenta;
        [Tooltip("The rate at which party HP ticks down (seconds) E.g. 0.05 = take away 1 health every 0.05 seconds")]
        public float BaseDecayRate = 0.05f;
        [Tooltip("How long to wait with combat dialog before moving to the next dialog line")]
        public float ActionLineSwitchRate = 0.25f;

        [Header("Dialog Settings")]

        public string introTxt = Utility.strIntroTxt;
        public string attackTxt = Utility.attackTxt;
        public string dmgTxt = Utility.dmgTxt;
        public string critTxt = Utility.critTxt;
        public string missTxt = Utility.missTxt;
        public string dodgeTxt = Utility.dodgeTxt;
        public string enemyDeathTxt = Utility.enemyDeathTxt;
        public string partyDeathTxt = Utility.partyDeathTxt;

        #endregion

        #region Public Variables
        [HideInInspector]
        public List<PartyEntity> PartyEntities = new List<PartyEntity>();
        [HideInInspector]
        public List<EnemyEntity> EnemyEntities = new List<EnemyEntity>();

        [HideInInspector]
        public ePartySlot CurrPartySlot;
        [HideInInspector]
        public eEnemySlot CurrEnemySelect;

        [HideInInspector]
        public EntityCommand CurrentCommand;      // The current command being formulated that will be added into the command list
        #endregion

        #region Private Variables
        List<TECF.EntityCommand> _commandList = new List<TECF.EntityCommand>();
        #endregion

        private void OnEnable()
        {
            EventManager.StartListening("NextPartyMember", OnNextPartyMember);
            EventManager.StartListening("ConfirmEnemySelect", OnConfirmEnemySelect);
            EventManager.StartListening("RunThroughCommands", OnRunThroughCommands);
            EventManager.StartListening("PartyUnconscious", OnPartyUnconscious);
            EventManager.StartListening("TameEnemy", OnTameEnemy);
        }

        private void OnDisable()
        {
            EventManager.StopListening("NextPartyMember", OnNextPartyMember);
            EventManager.StopListening("ConfirmEnemySelect", OnConfirmEnemySelect);
            EventManager.StopListening("RunThroughCommands", OnRunThroughCommands);
            EventManager.StopListening("PartyUnconscious", OnPartyUnconscious);
            EventManager.StopListening("TameEnemy", OnTameEnemy);
        }

        public void OnValidate()
        {
            // Force number of party members to max of 4
            if (PartyMembers.Length > 4)
            {
                System.Array.Resize(ref PartyMembers, 4);
                Debug.LogWarning("Party members cannot exceed max of 4 members!");
            }
        }

        private void Start()
        {
            CurrPartySlot = ePartySlot.NONE;

            /// Priority enemy
            BattleProfile priorityEnemy = Enemies[0];  // Enemy in first slot influences visuals and audio of whole battle

            Debug.Assert(priorityEnemy, "BATTLEMANAGER::No enemies set!");

            // Set enemy background
            ReferenceManager.Instance.enemyBG.sprite = priorityEnemy.BattleBackground;

            // Add enemy audio to main system and secondary systems
            ReferenceManager.Instance.mainAudio.AudioFiles.AddRange(priorityEnemy.BattleSFX);
            ReferenceManager.Instance.secondaryAudio.AudioFiles.AddRange(priorityEnemy.BattleSFX);

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
            for (int i = 0; i < Enemies.Length; ++i)
            {
                GameObject eObj = Instantiate(EnemyBlueprint);
                EnemyEntity eEe = eObj.GetComponent<EnemyEntity>();

                // Set to relevant slot
                eEe.enemySlot = (eEnemySlot)i;

                // Set type
                eEe.EntityType = eEntityType.ENEMY;

                // Assign battle profile
                eEe.BattleProfile = Enemies[i];

                // Keep track of enemy entity
                EnemyEntities.Add(eEe);

                eObj.transform.SetParent(ReferenceManager.Instance.enemyPanel.transform);
            }

            /// Party members
            for (int i = 0; i < PartyMembers.Length; ++i)
            {
                GameObject pmObj = Instantiate(PartyBlueprint);
                PartyEntity pmPe = pmObj.GetComponentInChildren<PartyEntity>();

                // Assign battle profile
                pmPe.BattleProfile = PartyMembers[i];

                // Set type
                pmPe.EntityType = eEntityType.PARTY;

                // Set to relevant slot
                pmPe.partySlot = (ePartySlot)i;

                // Keep track of party entity
                PartyEntities.Add(pmPe);

                pmObj.transform.SetParent(ReferenceManager.Instance.partyPanel.transform);
            }
        }

        void OnTameEnemy(IEventInfo a_info)
        {
            // Enemy was the last, freeze hp countdown on party members
            if (EnemyEntities.Count == 1)
            {
                foreach (var party in PartyEntities)
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
                if (partyInfo.partySlot == CurrPartySlot)
                {
                    EventManager.TriggerEvent("NextPartyMember");
                }

                /// Party member has fainted, tint the whole screen and activate death highlights
                var actionUI = ReferenceManager.Instance.actionPanel.GetComponentsInChildren<Image>();

                foreach (var aui in actionUI)
                {
                    aui.color = DeathTint;
                }

                var dialogUI = ReferenceManager.Instance.dialogPanel.GetComponentsInChildren<Image>();

                foreach (var dui in dialogUI)
                {
                    dui.color = DeathTint;
                }

                var partyUI = ReferenceManager.Instance.partyPanel.GetComponentsInChildren<Image>();

                foreach (var pui in partyUI)
                {
                    if (pui.CompareTag("Highlight"))
                    {
                        pui.color = DeathHighlight;
                    }
                    else
                    {
                        pui.color = DeathTint;
                    }
                }
            }
        }

        void OnRunThroughCommands(IEventInfo a_info)
        {
            // Sort list by sender turn order (highest to lowest)
            _commandList.Sort((a, b) => -(CalcTurnVal(a.sender.BattleProfile.Speed).CompareTo(CalcTurnVal(b.sender.BattleProfile.Speed))));

            // Go through command list and execute appropriate actions
            foreach (var cmd in _commandList)
            {
                switch (cmd.cmdType)
                {
                    case TECF.eCommandType.ATTACK:
                        /// Get ready function
                        PartyEntity party = cmd.sender as PartyEntity;
                        UnityAction partyReadyFunc = null;

                        // Party is attacking, ready them
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
                            dialogType = eDialogType.ATTACKING,
                            senderEntity = cmd.sender,
                            targetEntity = cmd.target,
                            endDialogFuncDelay = ActionLineSwitchRate,
                            startDialogFunc = partyReadyFunc
                        });

                        /// 1. Check for miss
                        int missNum = Random.Range(0, 100);
                        int missChance = Mathf.RoundToInt(ChanceToMiss * 100);

                        // Missed attack
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
                        int dodgeNum = Random.Range(0, 100);
                        int dodgeChance = Mathf.RoundToInt(((2 * cmd.target.BattleProfile.Speed - cmd.sender.BattleProfile.Speed) / 500f) * 100);

                        // Dodge success
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
                        int firstCritChance = Mathf.RoundToInt((cmd.sender.BattleProfile.Guts / 500f) * 100);
                        int secondCritChance = Mathf.RoundToInt((1f / 20f) * 100);
                        int critChance = Mathf.Max(firstCritChance, secondCritChance);

                        // Crit success
                        if (critNum <= critChance)
                        {
                            DialogManager.Instance.AddToQueue(new DialogInfo
                            {
                                dialogType = TECF.eDialogType.CRITICAL_HIT,
                                senderEntity = cmd.sender,
                                targetEntity = cmd.target
                            });

                            attackDmg *= CriticalHitModifier;
                        }

                        /// 5. Send damage to the target after dialog finishes
                        DialogManager.Instance.AddToQueue(new DialogInfo
                        {
                            strData = attackDmg.ToString(),
                            dialogType = TECF.eDialogType.DAMAGED,
                            senderEntity = cmd.sender,
                            targetEntity = cmd.target,
                            startDialogFunc = () =>
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

            // Gone through all commands, clear the list
            _commandList.Clear();

            // Activate action phase dialog
            EventManager.TriggerEvent("RunDialog", new DialogRunInfo
            {
                onDialogCompleteFunc = () => { EventManager.TriggerEvent("EndActionPhase"); }
            });
        }

        public void OnRegisterAttack()
        {
            // Create current command with known parameters so far (who is attacking, what kind of command it is)
            CurrentCommand = new TECF.EntityCommand { sender = GetPartyEntityBySlot(CurrPartySlot), cmdType = TECF.eCommandType.ATTACK };

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
            StartCoroutine("GetNextValidSlot");
        }

        /**
         * @brief Run through all enemies and decide what actions to take. (Should only be called after the player has made their turn)
         * */
        public void DecideEnemyTurns()
        {
            for (int i = 0; i < EnemyEntities.Count; ++i)
            {
                // Randomly pick a party member to attack
                PartyEntity targetPlayer = GetPartyEntityBySlot((ePartySlot)Random.Range(0, PartyMembers.Length));

                // Invalid target, find first conscious party member instead
                if (targetPlayer.CurrentStatus == eStatusEffect.UNCONSCIOUS)
                {
                    foreach (var party in PartyEntities)
                    {
                        if (party.CurrentStatus != eStatusEffect.UNCONSCIOUS)
                        {
                            targetPlayer = party;
                        }
                    }
                }

                EnemyEntity senderEnemy = EnemyEntities[i];

                _commandList.Add(new TECF.EntityCommand
                {
                    cmdType = TECF.eCommandType.ATTACK,
                    sender = senderEnemy,
                    target = targetPlayer
                });
            }
        }

        /**
         * @brief Find and return a party member that matches the given slot.
         * @param a_slot is the party slot to check.
         * @return Found party member or null if no slot matched.
         * */
        PartyEntity GetPartyEntityBySlot(ePartySlot a_slot)
        {
            foreach (var pe in PartyEntities)
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
            int percentModifier = (Random.Range(0, 2) == 0 ? 1 : -1);
            float halfVal = a_speed * 0.5f;

            return a_speed + halfVal * percentModifier;
        }

        /**
         * @brief Calculate the damage value that should be applied to the defender, also taking into account status effect modifiers.
         * @param a_status is the status effect of the attack.
         * @param a_attacker is the entity doing the damaging.
         * @param a_defender is the entity being damaged.
         * @return Amount of damage defender should take.
         * */
        float CalcDmgVal(eStatusEffect a_status, BattleEntity a_attacker, BattleEntity a_defender)
        {
            float dmg = 0;

            switch (a_status)
            {
                // Regular bash
                case eStatusEffect.NORMAL:
                    dmg += a_attacker.BattleProfile.Offense * (100f / (100f + (a_defender.BattleProfile.Defense)));
                    int percentModifier = (Random.Range(0, 2) == 0 ? 1 : -1);
                    dmg += dmg * 0.25f * percentModifier;
                    break;
                default:
                    break;
            }

            return dmg;
        }

        /**
         * @brief Find and set current slot to the first valid one found. If one cannot be found then the player's turn will end.
         * */
        IEnumerator GetNextValidSlot()
        {
            int prevSlot = (int)CurrPartySlot;
            int newSlot = prevSlot + 1;

            while (newSlot < PartyMembers.Length)
            {
                // Suggested member at slot is unconscious, skip to next
                if (GetPartyEntityBySlot((ePartySlot)newSlot).CurrentStatus == eStatusEffect.UNCONSCIOUS)
                {
                    newSlot++;
                }
                // Found valid member
                else
                {
                    CurrPartySlot = (ePartySlot)newSlot;

                    // Ready new party member
                    EventManager.TriggerEvent("OnPartyReady", new PartyInfo { partySlot = CurrPartySlot });

                    // No need to search for anymore slots
                    yield break;
                }

                yield return null;
            }

            // Next slot is outside bounds, assume player turn is over
            CurrPartySlot = ePartySlot.NONE;

            EventManager.TriggerEvent("EndPlayerTurn");
        }
    }
}