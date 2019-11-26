using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TECF
{
    public class BattleEntity : MonoBehaviour
    {
        [Header("References")]
        public Image EntityImage;

        #region Public Variables
        [HideInInspector]
        public eEntityType EntityType;
        [HideInInspector]
        public eStatusEffect CurrentStatus;     // Current affliction on the entity, will affect their behaviour if it isn't NORMAL
        [HideInInspector]
        public string EntityName;

        public virtual BattleProfile BattleProfile
        {
            get
            {
                return battleProfile;
            }
            set
            {
                battleProfile = value;

                // Set starting hp and power values
                Hp = battleProfile.Hp;
                Power = battleProfile.Power;

                var enemy = this as EnemyEntity;
                var party = this as PartyEntity;
                if (enemy)
                {
                    EntityName = enemy.battleProfile.EntityName + " " + enemy.enemySlot;
                }
                else
                {
                    EntityName = battleProfile.EntityName;
                }

                // Set sprite and scale
                if (EntityImage)
                {
                    EntityImage.sprite = battleProfile.BattleSprite;
                    EntityImage.rectTransform.sizeDelta = EntityImage.rectTransform.sizeDelta * battleProfile.EntityScale;
                }
            }
        }

        public virtual int Hp
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;

                // Clamp hp so it doesn't go below 0
                hp = Mathf.Max(hp, 0);
            }
        }

        public int Power
        {
            get
            {
                return power;
            }
            set
            {
                power = value;
            }
        }
        #endregion

        #region Inherited Variables
        protected BattleProfile battleProfile;
        protected int hp;
        protected int power;
        #endregion

        protected virtual void OnEnable()
        {
            EventManager.StartListening("TakeDamage", OnTakeDamage);
        }

        protected virtual void OnDisable()
        {
            EventManager.StopListening("TakeDamage", OnTakeDamage);
        }

        /**
         * @brief Convert number to display friendly version.
         * @return Display friendly version of number. E.g. 1 = {0,0,1} or 20 = {0,2,0}
         * */
        public int[] NumToDisplay(int a_num)
        {
            char[] numStr = a_num.ToString().ToCharArray();
            int digits = numStr.Length;
            int[] output = new int[] { 0, 0, 0 };

            // 1 digit
            switch (digits)
            {
                case 1:
                    output[0] = 0;
                    output[1] = 0;
                    output[2] = int.Parse(numStr[0].ToString());
                    break;
                case 2:
                    output[0] = 0;
                    output[1] = int.Parse(numStr[0].ToString());
                    output[2] = int.Parse(numStr[1].ToString());
                    break;
                case 3:
                    output[0] = int.Parse(numStr[0].ToString());
                    output[1] = int.Parse(numStr[1].ToString());
                    output[2] = int.Parse(numStr[2].ToString());
                    break;
                default:
                    output = (digits != 0) ? new int[] { 9, 9, 9 } : new int[] { 0, 0, 0 };
                    break;
            }

            return output;
        }

        protected virtual void OnTakeDamage(IEventInfo a_info)
        {
            DamageInfo dmgInfo = a_info as DamageInfo;

            // We are the target
            if (dmgInfo != null && dmgInfo.targetEntity == this)
            {
                // We or the attacker is unconscious, so ignore attack
                if (dmgInfo.senderEntity.CurrentStatus == eStatusEffect.UNCONSCIOUS ||
                    dmgInfo.targetEntity.CurrentStatus == eStatusEffect.UNCONSCIOUS)
                {
                    return;
                }

                DamageHealth(dmgInfo.dmg);
            }
        }

        protected virtual void DamageHealth(int a_dmg) { }
    }
}