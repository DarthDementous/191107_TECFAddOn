using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get { return m_stn; } }
    static BattleManager m_stn;

    [Tooltip("List of enemies for the party to fight. Battle music and background will be determined by the enemy in slot 0")]
    public TECF_BattleProfile[] enemies;

    [Tooltip("List of party members (maximum 4)")]
    public TECF_BattleProfile[] partyMembers = new TECF_BattleProfile[4];

    public ePartySlot currPartySlot;
    public eEnemySlot currEnemySelect;

    public float BaseDecayRate = 0.05f;

    private void OnEnable()
    {
        EventManager.StartListening("NextPartyMember", OnNextPartyMember);
        EventManager.StartListening("ConfirmEnemySelect", OnConfirmEnemySelect);
    }

    private void OnDisable()
    {
        EventManager.StopListening("NextPartyMember", OnNextPartyMember);
        EventManager.StopListening("ConfirmEnemySelect", OnConfirmEnemySelect);
    }

    void OnConfirmEnemySelect(IEventInfo a_info)
    {
        // TODO: Add attack information to the queue
        EventManager.TriggerEvent("NextPartyMember");
    }

    void OnNextPartyMember(IEventInfo a_info)
    {
        // Unready previous party member (if there was one)
        if (currPartySlot != ePartySlot.NONE) EventManager.TriggerEvent("PartyUnready", new PartyInfo { partySlot = currPartySlot });

        // Decide which party member should have the next turn
        if ((int)currPartySlot + 1 < partyMembers.Length)
        {
            currPartySlot++;
        }
        else
        {
            currPartySlot = ePartySlot.SLOT_1;
        }

        // Ready new party member
        EventManager.TriggerEvent("OnPartyReady", new PartyInfo { partySlot = currPartySlot });
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

            // Assign battle profile
            eEe.BattleProfile = enemies[i];

            // Set to relevant slot
            eEe.enemySlot = (eEnemySlot)i;

            eObj.transform.SetParent(ReferenceManager.Instance.enemyPanel.transform);
        }

        /// Party members
        for (int i = 0; i < partyMembers.Length; ++i)
        {
            GameObject pmObj        = Instantiate(Resources.Load("PartyMember") as GameObject);
            TECF_PartyEntity pmPe   = pmObj.GetComponentInChildren<TECF_PartyEntity>();

            // Assign battle profile
            pmPe.BattleProfile = partyMembers[i];

            // Set to relevant slot
            pmPe.partySlot = (ePartySlot)i;

            pmObj.transform.SetParent(ReferenceManager.Instance.partyPanel.transform);
        }
    }
}
