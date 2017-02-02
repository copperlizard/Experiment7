using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_levelDetails = new List<GameObject>();

    private int m_visibleDetails = 0;

	// Use this for initialization
	void Start ()
    {
        m_levelDetails[m_visibleDetails].SetActive(true);		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetVisibleDetails (int i)
    {
        if (i >= m_levelDetails.Count)
        {
            m_visibleDetails = 0;

            Debug.Log("i exceeds level index range! i == " + i.ToString());
        }
        else
        {
            m_levelDetails[m_visibleDetails].SetActive(false);
            m_visibleDetails = i;
            m_levelDetails[m_visibleDetails].SetActive(true);
        }    
    }

    public void LoadLevel ()
    {
        if (m_visibleDetails + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("no scene[" + (m_visibleDetails + 1).ToString() + "]; check build settings!");
        }
        else
        {
            SceneManager.LoadScene(m_visibleDetails + 1);
        }        
    }
}
