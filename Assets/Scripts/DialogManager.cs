using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{
    public string dialog;

    [Tooltip("How long to wait (in seconds) before displaying the next letter in the dialog paragraph.")]
    public float charDelay = 1f;

    public TextMeshProUGUI ref_dialogTxt;

    private void OnEnable()
    {
        EventManager.StartListening("OnDisplayDialog", CallDisplayDialog);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnDisplayDialog", CallDisplayDialog);
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(DisplayDialog(dialog, () => { EventManager.TriggerEvent("EndIntro"); }, 1));
    }

    void CallDisplayDialog(IEventInfo a_info)
    {
        DialogInfo dialogInfo = a_info as DialogInfo;

        if (dialogInfo != null)
        {
            StartCoroutine(DisplayDialog(dialogInfo.dialog, dialogInfo.endDialogFunc, dialogInfo.endDialogFuncDelay));
        }
    }

    /**
     * @brief Type out dialog into text box according to interval with optional callback once finished.
     * @param a_dialog is the string to type out.
     * @param a_callback is the function to call once the string is finished being typed out.
     * @param a_optDelayS is the optional time in seconds to wait before initiating the callback.
     * @return IEnumerator
     * */
    IEnumerator DisplayDialog(string a_dialog, UnityAction a_callback = null, float a_optDelayS = 0)
    {
        ref_dialogTxt.text = "";
        foreach (char letter in a_dialog.ToCharArray())
        {
            yield return new WaitForSeconds(charDelay);
            ref_dialogTxt.text += letter;
        }

        yield return new WaitForSeconds(a_optDelayS);

        // Activate callback if not null
        a_callback?.Invoke();

        // TODO: Make it so the end intro event only gets called for the intro dialog
        //EventManager.TriggerEvent("EndIntro");
    }
}
