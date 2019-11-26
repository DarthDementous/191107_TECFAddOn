using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace TECF
{
    public class EnemyEntity : BattleEntity
    {
        public EnemyEntity()
        {
            EntityType = eEntityType.ENEMY;
        }

        public eEnemySlot enemySlot;
        public Animator anim;

        bool isDefeated = false;

        public override int Hp
        {
            get
            {
                return base.Hp;
            }
            set
            {
                base.Hp = value;

                // Handle enemy defeat
                if (hp == 0 && isDefeated == false)
                {
                    DialogManager.Instance.AddToQueue(new DialogInfo
                    {
                        dialogType = TECF.eDialogType.FAINTED,
                        senderEntity = this,
                        endDialogFuncDelay = BattleManager.Instance.ActionLineSwitchRate,
                        startDialogFunc = () =>
                        {
                        // Let the battle manager know an enemy has been tamed
                        EventManager.TriggerEvent("TameEnemy");

                            CurrentStatus = eStatusEffect.UNCONSCIOUS;
                            OnTameEnemy();
                        }
                    }, true);

                    isDefeated = true;
                }
            }
        }

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            EventManager.StartListening("SelectEnemy", OnSelectEnemy);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventManager.StopListening("SelectEnemy", OnSelectEnemy);

        }

        protected override void DamageHealth(int a_dmg)
        {
            // Play hit animation
            anim.SetTrigger("Hit");

            Hp -= a_dmg;
        }

        void OnTameEnemy()
        {
            // Start death animation (will call destroy function)
            anim.SetTrigger("Death");
        }

        void OnDestroyEnemy()
        {
            // Remove from entity array
            BattleManager.Instance.EnemyEntities.Remove(this);

            // Destroy object
            Destroy(gameObject);
        }

        public void CallEvent(string a_eventName)
        {
            EventManager.TriggerEvent(a_eventName, new EnemyInfo { enemy = this });
        }

        void OnSelectEnemy(IEventInfo a_info)
        {
            EnemyInfo enemyInfo = a_info as EnemyInfo;

            if (enemyInfo != null && enemyInfo.enemy == this)
            {
                // Update selected name
                ReferenceManager.Instance.enemySelectPanelText.text = "To " + battleProfile.entityName + " " + enemySlot.ToString();

                // Update selected slot
                BattleManager.Instance.CurrEnemySelect = enemyInfo.enemy.enemySlot;

                // Update current command target (if there is one)
                if (BattleManager.Instance.CurrentCommand != null) BattleManager.Instance.CurrentCommand.target = this;

                // Ensure button is selected
                anim.SetBool("Selected", true);
            }
            // Ensure all other buttons are unselected
            else if (enemyInfo != null && enemyInfo.enemy.enemySlot != enemySlot)
            {
                anim.SetBool("Selected", false);
            }
        }
    }
}