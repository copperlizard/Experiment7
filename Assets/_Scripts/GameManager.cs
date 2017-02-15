using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Animator m_pauseMenuAnimator;

    private float m_timeElapsed;

    private bool m_paused = false, m_finished = false;

    // Use this for initialization
    void Start()
    {
        if (m_pauseMenuAnimator == null)
        {
            Debug.Log("m_pauseMenuAnimator not assigned!");
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

    public void LevelFinished ()
    {
        m_finished = true;
    }

    public float GetTimeElapsed ()
    {
        return m_timeElapsed;
    }
}
