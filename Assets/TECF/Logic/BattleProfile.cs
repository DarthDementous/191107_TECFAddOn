using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TECF
{
    [CreateAssetMenu(menuName = "TECF/BattleProfile")]
    public class BattleProfile : ScriptableObject
    {
        [Header("Entity Settings")]

        [Tooltip("Name to display in combat")]
        public string EntityName;
        [Tooltip("How much the entity should be scaled, scales the default dimensions of 256x256")]
        [Range(1f, 10f)]
        public float EntityScale = 1f;
        [Tooltip("Visual representation of the enemy in combat.")]
        public Sprite BattleSprite;
        [Tooltip("Static image to use in the background if fighting the entity.")]
        public Sprite BattleBackground;
        [Tooltip("Set of audio cues to use in battle." +
            "\nINTRO=Track to play when combat starts if entity is enemy. If not set then LOOP will be used instead." +
            "\nLOOP=Track to play on loop during combat if entity is enemy.")]
        public List<AudioSystem.AudioNode> BattleSFX = new List<AudioSystem.AudioNode>();

        [Header("Stats")]

        [Tooltip("Hit points, if this reaches 0 then the entity will be defeated.")]
        public int Hp;
        [Tooltip("Power points, this will reduce when the entity uses power moves.")]
        public int Power;
        [Tooltip("Determines how much damage is dealt with regular attacks")]
        public int Offense;
        [Tooltip("Lowers damage from attacks.")]
        public int Defense;
        [Tooltip("Determines who goes first in battle and chance of dodging an attack.")]
        public int Speed;
        [Tooltip("Increases the chance of dealing a critical attack with a regular attack.")]
        public int Guts;
    }
}