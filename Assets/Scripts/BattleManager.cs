using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    [Tooltip("List of enemies for the party to fight. Battle music and background will be determined by the enemy in slot 0")]
    public TECF_BattleProfile[] enemies;

    [Tooltip("List of party members (maximum 4)")]
    public TECF_BattleProfile[] partyMembers = new TECF_BattleProfile[4];

    public void OnValidate()
    {
        // Force number of party members to max of 4
        if (partyMembers.Length > 4)
        {
            System.Array.Resize(ref partyMembers, 4);
            Debug.LogWarning("Party members cannot exceed max of 4 members!");
        }
    }

    private void Start()
    {
        /// Enemies
        TECF_BattleProfile priorityEnemy = enemies[0];  // Enemy in first slot influences visuals and audio of whole battle

        Debug.Assert(priorityEnemy, "BATTLEMANAGER::No enemies set!");

        // Set enemy sprite
        ReferenceManager.Instance.enemyImg.sprite = priorityEnemy.battleSprite;

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

        /// Party members
        foreach (var pm in partyMembers)
        {
            GameObject pmObj        = Instantiate(Resources.Load("PartyMember") as GameObject);
            TECF_BattleEntity pmBe  = pmObj.GetComponent<TECF_BattleEntity>();

            // Assign battle profile
            pmBe.BattleProfile = pm;

            pmObj.transform.SetParent(ReferenceManager.Instance.partyPanel.transform);
        }
    }
}
