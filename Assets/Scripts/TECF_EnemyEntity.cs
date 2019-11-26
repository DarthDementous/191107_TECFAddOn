using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TECF_EnemyEntity : TECF_BattleEntity
{
    public TECF_EnemyEntity() {
        entityType = eEntityType.ENEMY;
    }

    public eEnemySlot enemySlot;
   
    Animator anim;
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
                    startDialogFunc = ()=>
                    {
                        // Let the battle manager know an enemy has been tamed
                        EventManager.TriggerEvent("TameEnemy");

                        currentStatus = eStatusEffect.UNCONSCIOUS;
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

        EventManager.StartListening("EnemySelecting", OnEnemySelecting);
        EventManager.StartListening("EnemyUnselecting", OnEnemyUnselecting);
        EventManager.StartListening("SelectEnemy", OnSelectEnemy);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        EventManager.StopListening("EnemySelecting", OnEnemySelecting);
        EventManager.StopListening("EnemyUnselecting", OnEnemyUnselecting);
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
        BattleManager.Instance.enemyEntities.Remove(this);

        // Destroy object
        Destroy(gameObject);
    }

    public void CallEvent(string a_eventName)
    {
        EventManager.TriggerEvent(a_eventName, new EnemyInfo { enemySlot = this.enemySlot});
    }

    void OnSelectEnemy(IEventInfo a_info)
    {
        EnemyInfo enemyInfo = a_info as EnemyInfo;

        if (enemyInfo != null && enemyInfo.enemySlot == enemySlot)
        {
            // Update selected name
            ReferenceManager.Instance.enemySelectPanelText.text = "To " + battleProfile.entityName + " " + enemySlot.ToString();

            // Update selected slot
            BattleManager.Instance.currEnemySelect = enemyInfo.enemySlot;

            // Update current command target (if there is one)
            if (BattleManager.Instance.CurrentCommand != null) BattleManager.Instance.CurrentCommand.target = this;

            // Ensure button is pressed
            anim.SetTrigger("Pressed");
        }
        // Ensure other buttons are in the normal state
        else if (enemyInfo != null && enemyInfo.enemySlot != enemySlot)
        {
            anim.SetTrigger("Normal");
        }
    }

    void OnEnemySelecting(IEventInfo a_info)
    {
        anim.SetBool("Selecting", true);
    }

    void OnEnemyUnselecting(IEventInfo a_info)
    {
        anim.SetBool("Selecting", false);
    }
}