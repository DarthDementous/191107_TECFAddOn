using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TECF_EnemyEntity : TECF_BattleEntity
{
    public TECF_EnemyEntity() { entityType = eEntityType.ENEMY; }

    public eEnemySlot enemySlot;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.StartListening("EnemySelecting", OnEnemySelecting);
        EventManager.StartListening("EnemyUnselecting", OnEnemyUnselecting);
        EventManager.StartListening("SelectEnemy", OnSelectEnemy);
    }

    private void OnDisable()
    {
        EventManager.StopListening("EnemySelecting", OnEnemySelecting);
        EventManager.StopListening("EnemyUnselecting", OnEnemyUnselecting);
        EventManager.StopListening("SelectEnemy", OnSelectEnemy);

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