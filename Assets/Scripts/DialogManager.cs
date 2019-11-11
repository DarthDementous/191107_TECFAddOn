using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public string dialog;

    [Tooltip("How long to wait (in seconds) before displaying the next letter in the dialog paragraph.")]
    public float charDelay = 1f;

    public TextMeshProUGUI ref_dialogTxt;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisplayDialog(dialog));
    }

    IEnumerator DisplayDialog(string a_dialog)
    {
        ref_dialogTxt.text = "";
        foreach (char letter in a_dialog.ToCharArray())
        {
            yield return new WaitForSeconds(charDelay);
            ref_dialogTxt.text += letter;
        }

        // TODO: Make it so the end intro event only gets called for the intro dialog
        EventManager.TriggerEvent("EndIntro");
    }
}
