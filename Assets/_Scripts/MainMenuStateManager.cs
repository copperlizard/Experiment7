using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainMenuState
{
  MainMenu, SettingsMenu, CampaignMenu, LevelDetails  
};

public class MainMenuStateManager : MonoBehaviour
{
    static private MainMenuStateManager m_this;

    public MainMenuState m_mainMenuState;

    public int m_selectedLevel = 0;

	// Use this for initialization
	void Awake ()
    {
		if (m_this == null)
        {
            m_this = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
	}
}
