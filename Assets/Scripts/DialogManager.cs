using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get { return m_stn; } }
    static DialogManager m_stn;

    public List<DialogInfo> DialogQueue = new List<DialogInfo>();
    //public string dialog;

    [Tooltip("How long to wait (in seconds) before displaying the next letter in the dialog paragraph.")]
    public float charDelay = 1f;

    [Tooltip("How long to wait (in seconds) before going to next line of dialog. Can be overridden by individual dialog lines.")]
    public float lineDelay = 1f;

    public TextMeshProUGUI ref_dialogTxt;

    private void Awake()
    {
        // Only one instance allowed, destroy object
        if (m_stn != null && m_stn != this)
        {
            Debug.LogError("DIALOGMANAGER::Only one instance allowed of this script! Destroying object.");
            Destroy(this.gameObject);
        }
        else
        {
            m_stn = this;
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("RunDialog", CallRunDialog);
    }

    private void OnDisable()
    {
        EventManager.StopListening("RunDialog", CallRunDialog);
    }

    void CallRunDialog(IEventInfo a_info)
    {
        DialogRunInfo dialogRunInfo = a_info as DialogRunInfo;

        if (dialogRunInfo != null)
        {
            StartCoroutine("RunThroughDialog", dialogRunInfo.onDialogCompleteFunc);
        }
        else
        {
            StartCoroutine(RunThroughDialog());
        }
    }

    /**
     * @brief Add dialog to queue with optional parameters.
     * @param a_dialogInfo is the dialog info item to add into the queue.
     * @param a_isPriority allows the dialog info item to skip to the front of the queue (e.g. death messages)
     * */
    public void AddToQueue(DialogInfo a_dialogInfo, bool a_isPriority = false)
    {
        a_dialogInfo.queueTime = (a_isPriority) ? 0 : Time.time;

        DialogQueue.Add(a_dialogInfo);
    }

    /**
     * @brief Go through all lines of dialog that are currently in the queue, including optional end line functions and delays
     * @param a_funcOnComplete is the optional function to run once all the dialog has been run through.
     * */
    IEnumerator RunThroughDialog(UnityAction a_funcOnComplete = null)
    {
        while (DialogQueue.Count != 0)
        {
            // Queue could've changed so make sure we sort it so most recent items are first (smallest time to biggest time)
            DialogQueue.Sort((a, b) => a.queueTime.CompareTo(b.queueTime));

            // Get dialog line info
            DialogInfo dialogInfo = DialogQueue[0]; DialogQueue.RemoveAt(0);

            // Optional start callback
            dialogInfo.startDialogFunc?.Invoke();

            // Gradually display text
            ref_dialogTxt.text = "";
            foreach (char letter in dialogInfo.dialog.ToCharArray())
            {
                yield return new WaitForSeconds(charDelay);
                ref_dialogTxt.text += letter;
            }

            // Optional delay (or use base delay)
            yield return new WaitForSeconds((dialogInfo.endDialogFuncDelay == 0f) ? lineDelay : dialogInfo.endDialogFuncDelay);

            // Optional end callback
            dialogInfo.endDialogFunc?.Invoke();
        }

        // Optional dialog end callback
        a_funcOnComplete?.Invoke();
    }

    //// Start is called before the first frame update
    //void Start()
    //{
    //    //StartCoroutine(DisplayDialog(dialog, () => { EventManager.TriggerEvent("EndIntro"); }, 1));
    //}

    //void Dialog(IEventInfo a_info)
    //{
    //    DialogInfo dialogInfo = a_info as DialogInfo;

    //    if (dialogInfo != null)
    //    {
    //        StartCoroutine(DisplayDialog(dialogInfo.dialog, dialogInfo.endDialogFunc, dialogInfo.endDialogFuncDelay));
    //    }
    //}

    ///**
    // * @brief Type out dialog into text box according to interval with optional callback once finished.
    // * @param a_dialog is the string to type out.
    // * @param a_callback is the function to call once the string is finished being typed out.
    // * @param a_optDelayS is the optional time in seconds to wait before initiating the callback.
    // * @return IEnumerator
    // * */
    //IEnumerator DisplayDialog(string a_dialog, UnityAction a_callback = null, float a_optDelayS = 0)
    //{
    //    ref_dialogTxt.text = "";
    //    foreach (char letter in a_dialog.ToCharArray())
    //    {
    //        yield return new WaitForSeconds(charDelay);
    //        ref_dialogTxt.text += letter;
    //    }

    //    yield return new WaitForSeconds(a_optDelayS);

    //    // Activate callback if not null
    //    a_callback?.Invoke();

    //    // Display next dialog line

    //    // TODO: Make it so the end intro event only gets called for the intro dialog
    //    //EventManager.TriggerEvent("EndIntro");
    //}
}
