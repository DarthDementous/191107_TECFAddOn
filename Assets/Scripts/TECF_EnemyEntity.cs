using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TECF_EnemyEntity : TECF_BattleEntity
{
    public TECF_EnemyEntity() { entityType = eEntityType.ENEMY; }

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.StartListening("EnemySelect", OnEnemySelect);
        EventManager.StartListening("EnemyUnselect", OnEnemyUnselect);
    }

    private void OnDisable()
    {
        EventManager.StopListening("EnemySelect", OnEnemySelect);
        EventManager.StopListening("EnemySelect", OnEnemyUnselect);
    }

    void OnEnemySelect(IEventInfo a_info)
    {
        anim.SetBool("Selecting", true);
    }

    void OnEnemyUnselect(IEventInfo a_info)
    {
        anim.SetBool("Selecting", false);
    }
}