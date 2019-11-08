using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TECF/BattleProfile")]
public class TECF_BattleProfile : ScriptableObject
{
    [Tooltip("Name to display in combat")]
    public string entityName;

    [Tooltip("Visual representation of the enemy in combat.")]
    public Sprite battleSprite;
    [Tooltip("Static image to use in the background if fighting the entity.")]
    public Sprite battleBG;

    [Tooltip("Set of audio cues to use in battle." +
        "\nINTRO=Track to play when combat starts if entity is enemy. If not set then LOOP will be used instead." +
        "\nLOOP=Track to play on loop during combat if entity is enemy.")]    // TODO: List names of pre-set audio cues
    public List<AudioSystem.AudioNode> battleSFX = new List<AudioSystem.AudioNode>();

    [Tooltip("Hit points, if this reaches 0 then the entity will be defeated.")]
    public int hp;
    [Tooltip("Power points, this will reduce when the entity uses power moves.")]
    public int power;
    [Tooltip("Increases the chance of dealing a critical attack with a regular attack.")]
    public int guts;
    [Tooltip("Increases the chance an attack is dodged.")]
    public int luck;
    [Tooltip("Determines who goes first in battle.")]
    public int speed;
    [Tooltip("Determines how much damage is dealt with regular attacks")]
    public int offense;
    [Tooltip("Lowers damage from attacks.")]
    public int defense;

    // TODO: Add list of powers
}
