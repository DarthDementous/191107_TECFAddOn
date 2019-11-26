using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Linq;

namespace TECF
{
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

        public bool IsWriting   // Whether or not dialog is in the process of being written
        {
            get
            {
                return _isWriting;
            }
            set
            {
                _isWriting = value;

                // Update dialog visuals
                ref_visuals.SetActive(_isWriting);

                // Update action panel visibility
                ReferenceManager.Instance.actionPanel.SetActive(!_isWriting);
            }
        }
        bool _isWriting;

        public TextMeshProUGUI ref_dialogTxt;
        public GameObject ref_visuals;

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

        public void PollForFaint()
        {
            // Check if there are any faint messages in the queue and run them early (if not running already)
            if (IsWriting == false)
            {
                foreach (var d in DialogQueue)
                {
                    if (d.dialogType == TECF.eDialogType.FAINTED && d.senderEntity.EntityType == eEntityType.PARTY)
                    {
                        StartCoroutine(RunThroughDialog());
                        break;
                    }
                }
            }
        }

        void CallRunDialog(IEventInfo a_info)
        {
            DialogRunInfo dialogRunInfo = a_info as DialogRunInfo;

            StopAllCoroutines();

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
            // Figure out final dialog string from type (if given)
            switch (a_dialogInfo.dialogType)
            {
                case TECF.eDialogType.INTRO:
                    a_dialogInfo.SetDialog(BattleManager.Instance.introTxt + " " + a_dialogInfo.strData);
                    break;
                case TECF.eDialogType.ATTACKING:
                    a_dialogInfo.SetDialog(a_dialogInfo.senderEntity.EntityName + " " + BattleManager.Instance.attackTxt);
                    break;
                case TECF.eDialogType.CRITICAL_HIT:
                    a_dialogInfo.SetDialog(BattleManager.Instance.critTxt);
                    break;
                case TECF.eDialogType.FAINTED:
                    string faintTxt = (a_dialogInfo.senderEntity.EntityType == eEntityType.ENEMY) ? BattleManager.Instance.enemyDeathTxt : BattleManager.Instance.partyDeathTxt;

                    a_dialogInfo.SetDialog(a_dialogInfo.senderEntity.EntityName + " " + faintTxt);
                    break;
                case TECF.eDialogType.MISS:
                    a_dialogInfo.SetDialog(BattleManager.Instance.missTxt);
                    break;
                case TECF.eDialogType.DODGED:
                    a_dialogInfo.SetDialog(a_dialogInfo.targetEntity.EntityName + " " + BattleManager.Instance.dodgeTxt);
                    break;
                case TECF.eDialogType.DAMAGED:
                    a_dialogInfo.SetDialog(a_dialogInfo.strData + " " + BattleManager.Instance.dmgTxt + " " + a_dialogInfo.targetEntity.EntityName);
                    break;
                default:
                    a_dialogInfo.SetDialog(a_dialogInfo.strData);
                    break;
            }

            // Not priority, add to end of queue
            if (a_isPriority == false)
            {
                DialogQueue.Add(a_dialogInfo);
            }
            // Is priority, add to start of queue
            else
            {
                DialogQueue.Insert(0, a_dialogInfo);
            }
        }

        /**
         * @brief Go through all lines of dialog that are currently in the queue, including optional end line functions and delays
         * @param a_funcOnComplete is the optional function to run once all the dialog has been run through.
         * */
        IEnumerator RunThroughDialog(UnityAction a_funcOnComplete = null)
        {
            IsWriting = true;

            while (DialogQueue.Count != 0)
            {
                // Get dialog line info
                DialogInfo dialogInfo = DialogQueue[0]; DialogQueue.RemoveAt(0);

                // Validate that dialog is still valid or else skip
                if (!IsDialogValid(dialogInfo))
                {
                    continue;
                }

                // Optional start callback
                dialogInfo.startDialogFunc?.Invoke();

                // Gradually display text
                ref_dialogTxt.text = "";
                foreach (char letter in dialogInfo.GetDialog().ToCharArray())
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

            IsWriting = false;
        }

        bool IsDialogValid(DialogInfo a_dialog)
        {
            // Check if dialog type is combat
            if (a_dialog.dialogType > TECF.eDialogType.COMBAT && a_dialog.dialogType < TECF.eDialogType.COMBAT_END)
            {
                // Null check
                if (a_dialog.senderEntity == null || a_dialog.targetEntity == null)
                {
                    return false;
                }

                // Unconscious check
                if (a_dialog.senderEntity.CurrentStatus == eStatusEffect.UNCONSCIOUS ||
                    a_dialog.targetEntity.CurrentStatus == eStatusEffect.UNCONSCIOUS)
                {
                    return false;
                }
            }

            return true;
        }
    }
}