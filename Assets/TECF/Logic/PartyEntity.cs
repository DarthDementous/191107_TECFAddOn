using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TECF
{
    public class PartyEntity : BattleEntity
    {
        public PartyEntity() { EntityType = eEntityType.PARTY; }

        #region Public Facing Variables
        [Header("References")]

        public TextMeshProUGUI HealthText;
        public TextMeshProUGUI PowerText;
        public Text NameText;
        public GameObject HpObj;
        public GameObject PPObj;

        [Header("Frame Settings")]

        [Tooltip("How high up to move the party frame when in the ready position.")]
        public float readyOffset = 50f;
        #endregion

        #region Public Variables
        public override BattleProfile BattleProfile
        {
            get
            {
                return base.BattleProfile;
            }
            set
            {
                base.BattleProfile = value;

                // Set name
                if (NameText) NameText.text = battleProfile.EntityName;
            }
        }

        public override int Hp
        {
            get
            {
                return base.Hp;
            }
            set
            {
                base.Hp = value;

                // Set visual counter values to hp
                if (HpObj)
                {
                    var counterHP = NumToDisplay(hp);
                    var counterVals = HpObj.GetComponentsInChildren<NumberScroller>();

                    for (int i = 0; i < counterVals.Length; ++i)
                    {
                        counterVals[i].SetTargetNum(counterHP[i], true);
                    }
                }

                // Handle party defeat
                if (hp == 0 && b_isFainted == false)
                {
                    DialogManager.Instance.AddToQueue(new DialogInfo
                    {
                        dialogType = TECF.eDialogType.FAINTED,
                        senderEntity = this,
                        endDialogFunc = () =>
                        {
                            CurrentStatus = eStatusEffect.UNCONSCIOUS;
                        }
                    }, true);

                    b_isFainted = true;
                }
            }
        }
        #endregion

        [HideInInspector]
        public ePartySlot partySlot;

        bool b_isFainted;

        protected override void DamageHealth(int a_dmg)
        {
            base.DamageHealth(a_dmg);

            StartCoroutine(TickDownHealth(a_dmg));
        }

        IEnumerator TickDownHealth(int a_dmg)
        {
            int targetHealth = Hp - a_dmg;

            while (Hp > targetHealth)
            {
                Hp--;

                yield return new WaitForSeconds(BattleManager.Instance.BaseDecayRate);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            EventManager.StartListening("OnPartyReady", OnPartyReady);
            EventManager.StartListening("PartyUnready", OnPartyUnready);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventManager.StopListening("OnPartyReady", OnPartyReady);
            EventManager.StopListening("PartyUnready", OnPartyUnready);
        }

        void OnPartyUnready(IEventInfo a_info)
        {
            PartyInfo partyInfo = a_info as PartyInfo;

            if (partyInfo != null && partyInfo.partySlot == partySlot || partyInfo.partySlot == ePartySlot.NONE)
            {
                // Reset position to default
                gameObject.transform.localPosition = Vector3.zero;
            }
        }

        void OnPartyReady(IEventInfo a_info)
        {
            PartyInfo partyInfo = a_info as PartyInfo;

            if (partyInfo != null && partyInfo.partySlot == partySlot)
            {
                //Debug.Log("ON PARTY READY FOR " + partyInfo.partySlot);

                // There can only be one party ready at a time, so unready all others
                for (int i = 0; i < BattleManager.Instance.PartyMembers.Length; ++i)
                {
                    ePartySlot currSlot = (ePartySlot)i;

                    if (partyInfo.partySlot != currSlot)
                    {
                        EventManager.TriggerEvent("PartyUnready", new PartyInfo { partySlot = currSlot });
                    }
                }

                // Update action panel data
                ReferenceManager.Instance.actionPanelName.text = battleProfile.EntityName;

                // Visually move into ready position
                gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, readyOffset, gameObject.transform.localPosition.z);
            }
        }
    }
}