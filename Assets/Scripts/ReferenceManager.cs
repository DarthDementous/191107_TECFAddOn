using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get { return m_stn; } }

    static ReferenceManager m_stn;

    //public Image enemyImg;      // TODO: Add support for multiple enemy images
    public Image enemyBG;

    public AudioSystem mainAudio;
    public AudioSystem secondaryAudio;

    public GameObject partyPanel;
    public GameObject dialogPanel;
    public GameObject actionPanel;
    public GameObject enemyPanel;

    public Text actionPanelName;

    private void Awake()
    {
        // Only one instance allowed, destroy object
        if (m_stn != null && m_stn != this)
        {
            Debug.LogError("REFERENCEMANAGER::Only one instance allowed of this script! Destroying object.");
            Destroy(this.gameObject);
        }
        else
        {
            m_stn = this;
        }
    }
}
