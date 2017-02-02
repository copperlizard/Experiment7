using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_mainMenuPanel, m_settingsPanel, m_campaignPanel, m_levelDetailsPanel;

    private Animator m_mainMenuAnimator, m_settingsAnimator, m_campaignAnimator, m_levelDetailsAnimator;

	// Use this for initialization
	void Start ()
    {
	    if (m_mainMenuPanel == null)
        {
            Debug.Log("m_mainMenuPanel not assigned!");
            return;
        }

        m_mainMenuAnimator = m_mainMenuPanel.GetComponent<Animator>();
        if (m_mainMenuAnimator == null)
        {
            Debug.Log("could not find m_mainMenuAnimator!");
            return;
        }

        if (m_settingsPanel == null)
        {
            Debug.Log("m_settingsPanel not assigned!");
            return;
        }

        m_settingsAnimator = m_settingsPanel.GetComponent<Animator>();
        if (m_settingsAnimator == null)
        {
            Debug.Log("could not find m_settingsAnimator!");
            return;
        }

        if (m_campaignPanel == null)
        {
            Debug.Log("m_campaignPanel not assigned!");
            return;
        }

        m_campaignAnimator = m_campaignPanel.GetComponent<Animator>();
        if (m_campaignAnimator == null)
        {
            Debug.Log("could not find m_campaignAnimator!");
            return;
        }

        if (m_levelDetailsPanel == null)
        {
            Debug.Log("m_levelDetailsPanel not assigned!");
            return;
        }

        m_levelDetailsAnimator = m_levelDetailsPanel.GetComponent<Animator>();
        if (m_levelDetailsAnimator == null)
        {
            Debug.Log("could not find m_levelDetailsAnimator!");
            return;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OpenMainMenu ()
    {
        m_mainMenuAnimator.SetBool("visible", true);
        m_settingsAnimator.SetBool("visible", false);
        m_campaignAnimator.SetBool("visible", false);
        m_levelDetailsAnimator.SetBool("visible", false);
    }

    public void OpenSettingsMenu()
    {
        m_mainMenuAnimator.SetBool("visible", false);
        m_settingsAnimator.SetBool("visible", true);
        m_campaignAnimator.SetBool("visible", false);
        m_levelDetailsAnimator.SetBool("visible", false);
    }

    public void OpenCampaignMap()
    {
        m_mainMenuAnimator.SetBool("visible", false);
        m_settingsAnimator.SetBool("visible", false);
        m_campaignAnimator.SetBool("visible", true);
        m_levelDetailsAnimator.SetBool("visible", false);
    }

    public void OpenLevelDetails()
    {
        m_mainMenuAnimator.SetBool("visible", false);
        m_settingsAnimator.SetBool("visible", false);
        m_campaignAnimator.SetBool("visible", true);
        m_levelDetailsAnimator.SetBool("visible", true);
    }
}
