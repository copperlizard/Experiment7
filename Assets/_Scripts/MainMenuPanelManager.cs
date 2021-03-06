﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanelManager : MonoBehaviour
{    
    private MainMenuStateManager m_mainMenuStateManager;

    [SerializeField]
    private GameObject m_mainMenuPanel, m_settingsPanel, m_campaignPanel, m_levelDetailsPanel;

    private Animator m_mainMenuAnimator, m_settingsAnimator, m_campaignAnimator, m_levelDetailsAnimator;

    private MenuPanelInput m_campaignMenuInput;

	// Use this for initialization
	void Start ()
    {
        m_mainMenuStateManager = GameObject.FindGameObjectWithTag("MainMenuStateManager").GetComponent<MainMenuStateManager>();
        if (m_mainMenuStateManager == null)
        {
            Debug.Log("m_mainMenuStateManager not found!");
            return;
        }

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
            Debug.Log("m_levelDetailsAnimator not found!");
            return;
        }

        m_campaignMenuInput = m_campaignPanel.GetComponent<MenuPanelInput>();
        if (m_campaignMenuInput == null)
        {
            Debug.Log("m_campaignMenuInput not found!");
        }

        //Debug.Log("Start Panel Switch!");
        switch (m_mainMenuStateManager.m_mainMenuState)
        {
            case MainMenuState.CampaignMenu:
                OpenCampaignMap();
                //Debug.Log("OpenCampaignMap");
                break;
            case MainMenuState.LevelDetails:
                OpenLevelDetails();
                //Debug.Log("OpenLevelDetails");
                break;
            case MainMenuState.MainMenu:
                OpenMainMenu();
                //Debug.Log("OpenMainMenu");
                break;
            case MainMenuState.SettingsMenu:
                OpenSettingsMenu();
                //Debug.Log("OpenSettingsMenu");
                break;
            default:
                OpenMainMenu();
                //Debug.Log("Default"); ;
                break;
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

        m_mainMenuStateManager.m_mainMenuState = MainMenuState.MainMenu;
    }

    public void OpenSettingsMenu()
    {
        m_mainMenuAnimator.SetBool("visible", false);
        m_settingsAnimator.SetBool("visible", true);
        m_campaignAnimator.SetBool("visible", false);
        m_levelDetailsAnimator.SetBool("visible", false);

        m_mainMenuStateManager.m_mainMenuState = MainMenuState.SettingsMenu;
    }

    public void OpenCampaignMap()
    {
        m_mainMenuAnimator.SetBool("visible", false);
        m_settingsAnimator.SetBool("visible", false);
        m_campaignAnimator.SetBool("visible", true);
        m_levelDetailsAnimator.SetBool("visible", false);

        m_campaignMenuInput.ResetPanelSelection();

        m_mainMenuStateManager.m_mainMenuState = MainMenuState.CampaignMenu;
    }

    public void OpenLevelDetails()
    {
        m_mainMenuAnimator.SetBool("visible", false);
        m_settingsAnimator.SetBool("visible", false);
        m_campaignAnimator.SetBool("visible", true);
        m_levelDetailsAnimator.SetBool("visible", true);
        
        m_mainMenuStateManager.m_mainMenuState = MainMenuState.LevelDetails;
    }
}
