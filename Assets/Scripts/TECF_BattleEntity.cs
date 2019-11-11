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
    [Tooltip("NONE=Default, should never be set to this." +
        "\nENEMY=The player party will fight against them" +
        "PARTY=The player party will fight with them")]
    public eEntityType entityType;

    [Tooltip("Current affliction on the entity, will affect their behaviour if it isn't NORMAL")]
    public eStatusEffect currentStatus;

    public TextMeshProUGUI healthTxt;
    public TextMeshProUGUI powerTxt;
    public Text            nameTxt;

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
            nameTxt.text = battleProfile.entityName;
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

            // Set text to formatted version of hp
            healthTxt.text = NumToDisplay(hp);
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
            powerTxt.text = NumToDisplay(power);
        }
    }


    TECF_BattleProfile battleProfile;
    int hp;
    int power;

    [HideInInspector]
    StateManager m_stateManager;    // TODO: Get reference to state manager on object

    [HideInInspector]
    bool b_autoFight;   // Whether AI should take over for this entity or be player controlled

    /**
     * @brief Convert number to display friendly version.
     * @return Display friendly version of number. E.g. 120 = "1 2 0" or 20 = "0 2 0"
     * */
    public string NumToDisplay(int a_num)
    {
        char[] numStr   = a_num.ToString().ToCharArray();
        int digits      = numStr.Length;
        string output   = "";

        // 1 digit
        switch (digits)
        {
            case 1:
                output = "0 0 " + numStr[0];
                break;
            case 2:
                output = "0 " + numStr[0] + " " + numStr[1];
                break;
            case 3:
                output = numStr[0] + " " + numStr[1] + " " + numStr[2];
                break;
            default:
                output = (digits != 0) ? "9 9 9" : "0 0 0";
                break;
        }

        return output;
    }
}
