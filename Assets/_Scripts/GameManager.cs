using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Animator m_pauseMenuAnimator, m_levelCompleteMenuAnimator;

    [SerializeField]
    private Text m_recordsText;

    private MainMenuStateManager m_mainMenuStateManager;

    private DataManager m_dataManager;

    private float m_timeElapsed;

    private bool m_paused = false, m_finished = false;

    // Use this for initialization
    void Awake()
    {
        if (m_pauseMenuAnimator == null)
        {
            Debug.Log("m_pauseMenuAnimator not assigned!");
        }

        if (m_levelCompleteMenuAnimator == null)
        {
            Debug.Log("m_levelCompleteMenuAnimator not assigned!");
        }

        if (m_recordsText == null)
        {
            Debug.Log("m_recordsText not assigned!");
        }

        m_mainMenuStateManager = GameObject.FindGameObjectWithTag("MainMenuStateManager").GetComponent<MainMenuStateManager>();
        if (m_mainMenuStateManager == null)
        {
            Debug.Log("m_mainMenuStateManager not found!");
        }

        m_dataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataManager>();
        if (m_dataManager == null)
        {
            Debug.Log("m_dataManager not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_finished)
        {
            m_timeElapsed = Time.timeSinceLevelLoad;
        }        
    }
    
    public void PauseGame ()
    {
        if (m_paused)
        {
            ResumeGame();
            return;
        }

        Debug.Log("pause!");

        m_paused = true;
        m_pauseMenuAnimator.SetBool("visible", true);

        Time.timeScale = 0.0f;
    }

    public void ResumeGame ()
    {
        Debug.Log("resume!");

        m_paused = false;
        m_pauseMenuAnimator.SetBool("visible", false);

        Time.timeScale = 1.0f;
    }

    public bool IsPaused ()
    {
        return m_paused;
    }

    public void ReloadScene ()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene (int ind)
    {
        SceneManager.LoadScene(ind);
    }

    public void SetMainMenuState(int i)
    {
        switch (i)
        {
            case 0:
                m_mainMenuStateManager.m_mainMenuState = MainMenuState.MainMenu;
                break;
            case 1:
                m_mainMenuStateManager.m_mainMenuState = MainMenuState.CampaignMenu;
                break;
            case 2:
                m_mainMenuStateManager.m_mainMenuState = MainMenuState.LevelDetails;
                break;
            case 3:
                m_mainMenuStateManager.m_mainMenuState = MainMenuState.SettingsMenu;
                break;
        }        
    }

    public void LevelFinished ()
    {
        m_finished = true;

        List<float> recs = m_dataManager.UpdateRecords(m_timeElapsed);

        //Update records text
        string recText = "";

        foreach (float rec in recs)
        {
            TimeSpan t = TimeSpan.FromSeconds(rec);
            if (rec == m_timeElapsed)
            {
                recText += "<color=#FF7A7AFF>";
            }
            recText += string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds) + System.Environment.NewLine;
            if (rec == m_timeElapsed)
            {
                recText += "</color>";
            }
        }

        m_recordsText.text = recText;

        m_levelCompleteMenuAnimator.SetBool("visible", true);
    }

    public float GetTimeElapsed ()
    {
        return m_timeElapsed;
    }
}
