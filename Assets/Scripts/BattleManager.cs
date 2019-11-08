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
        TECF_BattleProfile priorityEnemy = enemies[0];  // Enemy in first slot influences visuals and audio of whole battle

        Debug.Assert(priorityEnemy, "BATTLEMANAGER::No enemies set!");

        // Set enemy sprite
        ReferenceManager.Instance.enemyImg.sprite = priorityEnemy.battleSprite;

        // Set enemy background
        ReferenceManager.Instance.enemyBG.sprite = priorityEnemy.battleBG;

        // Add enemy audio to main system
        ReferenceManager.Instance.mainAudio.audioFiles.AddRange(priorityEnemy.battleSFX);

        // Start battle music, playing intro first if there is one
        if (ReferenceManager.Instance.mainAudio.HasClip("INTRO"))
        {
            // TODO: Add logic for intro music here
        }
        else if (ReferenceManager.Instance.mainAudio.HasClip("LOOP"))
        {
            ReferenceManager.Instance.mainAudio.SetLooping(true);
            ReferenceManager.Instance.mainAudio.PlayClip("{\"alias\":\"LOOP\",\"volume\":1}");
        }
    }
}
