using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TECF_BattleEntity : MonoBehaviour
{
    public eEntityType entityType;

    [Tooltip("Current affliction on the entity, will affect their behaviour if it isn't NORMAL")]
    public eStatusEffect currentStatus;

    public string entityName;

    public TextMeshProUGUI healthTxt;
    public TextMeshProUGUI powerTxt;
    public Text            nameTxt;
    public Image           entityImg;
    public GameObject hpObj;
    public GameObject ppObj;

    public TECF_BattleProfile BattleProfile
    {
        get
        {
            return battleProfile;
        }
        set
        {
            battleProfile = value;

            // Set starting hp and power values
            Hp      = battleProfile.hp;
            Power   = battleProfile.power;

            // Set name
            if (nameTxt) nameTxt.text = battleProfile.entityName;

            var enemy = this as TECF_EnemyEntity;
            var party = this as TECF_PartyEntity;
            if (enemy)
            {
                entityName = enemy.battleProfile.entityName + " " + enemy.enemySlot;
            }
            else
            {
                entityName = battleProfile.entityName;
            }

            // Set sprite
            if (entityImg) entityImg.sprite = battleProfile.battleSprite;
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

            // Set text to formatted version of hp
            //if (powerTxt) powerTxt.text = NumToDisplay(power);
        }
    }

    protected TECF_BattleProfile battleProfile;
    protected int hp;
    protected int power;

    [HideInInspector]
    protected StateManager m_stateManager;    // TODO: Get reference to state manager on object

    [HideInInspector]
    protected bool b_autoFight;   // Whether AI should take over for this entity or be player controlled

    /**
     * @brief Convert number to display friendly version.
     * @return Display friendly version of number. E.g. 1 = {0,0,1} or 20 = {0,2,0}
     * */
    public int[] NumToDisplay(int a_num)
    {
        char[] numStr   = a_num.ToString().ToCharArray();
        int digits      = numStr.Length;
        int[] output    = new int[] { 0, 0, 0};

        // 1 digit
        switch (digits)
        {
            case 1:
                output[0] = 0;
                output[1] = 0;
                output[2] = int.Parse(numStr[0].ToString());
                //output = "00" + numStr[0];
                break;
            case 2:
                output[0] = 0;
                output[1] = int.Parse(numStr[0].ToString());
                output[2] = int.Parse(numStr[1].ToString());
                //output = "0" + numStr[0] + "" + numStr[1];
                break;
            case 3:
                output[0] = int.Parse(numStr[0].ToString());
                output[1] = int.Parse(numStr[1].ToString());
                output[2] = int.Parse(numStr[2].ToString());
                //output = "" + numStr[0] + "" + numStr[1] + "" + numStr[2];
                break;
            default:
                output = (digits != 0) ? new int[]{ 9,9,9} : new int[]{ 0,0,0};
                break;
        }

        return output;
    }

    protected virtual void OnEnable()
    {
        EventManager.StartListening("TakeDamage", OnTakeDamage);
        EventManager.StartListening("Heal", OnHeal);
    }

    protected virtual void OnDisable()
    {
        EventManager.StopListening("TakeDamage", OnTakeDamage);
        EventManager.StopListening("Heal", OnHeal);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    StartCoroutine("OnTakeDamage", 15);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    StartCoroutine("OnHeal", 37);
        //}
    }

    protected virtual void OnTakeDamage(IEventInfo a_info)
    {
        DamageInfo dmgInfo = a_info as DamageInfo;

        // We are the target
        if (dmgInfo != null && dmgInfo.targetEntity == this)
        {
            // We or the attacker is unconscious, so ignore attack
            if (dmgInfo.senderEntity.currentStatus == eStatusEffect.UNCONSCIOUS || 
                dmgInfo.targetEntity.currentStatus == eStatusEffect.UNCONSCIOUS)
            {
                return;
            }

            DamageHealth(dmgInfo.dmg);
        }
    }

    protected virtual void DamageHealth(int a_dmg) { }


    void OnHeal(IEventInfo a_info)
    {
        //int targetHealth = Hp + a_healAmount;

        //while (Hp < targetHealth)
        //{
        //    Hp++;

        //    yield return new WaitForSeconds(BattleManager.Instance.BaseDecayRate);
        //}
    }
}
