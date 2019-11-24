using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum eEntityType
{
    NONE,
    ENEMY,
    PARTY
}

public enum eStatusEffect       // TODO: Add explanations for status effects
{
    NORMAL,
    HEAL,
    ASLEEP,
    BLIND,
    BURNING,
    CONFUSED,
    UNCONSCIOUS
}

public class TECF_BattleEntity : MonoBehaviour
{
    public eEntityType entityType;

    [Tooltip("Current affliction on the entity, will affect their behaviour if it isn't NORMAL")]
    public eStatusEffect currentStatus;

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

            // Set sprite
            if (entityImg) entityImg.sprite = battleProfile.battleSprite;
        }
    }

    public int Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;

            // Set visual counter values to hp
            if (hpObj)
            {
                var counterHP = NumToDisplay(hp);
                var counterVals = hpObj.GetComponentsInChildren<NumberScroller>();

                for (int i = 0; i < counterVals.Length; ++i)
                {
                    counterVals[i].CurrentNum = counterHP[i];
                }
            }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine("OnTakeDamage", 50);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine("OnHeal", 15);
        }

        if (entityType == eEntityType.PARTY)
        {
            Debug.Log(Hp);
        }
    }

    IEnumerator OnTakeDamage(int a_dmg)
    {
        // TODO: Hook up to incoming damage

        int targetHealth = Hp - a_dmg;

        while (Hp > targetHealth)
        {
            Hp--;

            yield return new WaitForSeconds(BattleManager.Instance.BaseDecayRate);
        }
    }

    IEnumerator OnHeal(int a_healAmount)
    {
        int targetHealth = Hp + a_healAmount;

        while (Hp < targetHealth)
        {
            Hp++;

            yield return new WaitForSeconds(BattleManager.Instance.BaseDecayRate);
        }
    }
}
