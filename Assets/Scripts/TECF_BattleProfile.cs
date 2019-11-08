using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TECF_BattleProfile : ScriptableObject
{
    [Tooltip("Name to display in combat")]
    string battleName;

    [Tooltip("Visual representation of the enemy in combat.")]
    Sprite battleSprite;
    [Tooltip("Static image to use in the background if fighting the entity.")]
    Sprite battleBG;

    [Tooltip("Set of audio cues to use in battle." +
        "\nINTRO=Track to play when combat starts if entity is enemy. If not set then LOOP will be used instead." +
        "\nLOOP=Track to play on loop during combat if entity is enemy.")]    // TODO: List names of pre-set audio cues
    AudioSystem battleSFX;

    [Tooltip("Hit points, if this reaches 0 then the entity will be defeated.")]
    int hp;
    [Tooltip("Power points, this will reduce when the entity uses power moves.")]
    int power;
    [Tooltip("Increases the chance of dealing a critical attack with a regular attack.")]
    int guts;
    [Tooltip("Increases the chance an attack is dodged.")]
    int luck;
    [Tooltip("Determines who goes first in battle.")]
    int speed;
    [Tooltip("Lowers damage from attacks.")]
    int defense;
    [Tooltip("Determines how much damage is dealt with regular attacks")]
    int offense;

    // TODO: Add list of powers
}
