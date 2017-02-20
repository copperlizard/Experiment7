using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private Image m_speedGauge, m_excessSpeedGauge, m_dashGauge;

    [SerializeField]
    private Text m_speedText, m_timeText;

    private GameManager m_gameManager;

    private GameObject m_player;
    private PlayerController m_playerController;

	// Use this for initialization
	void Start ()
    {
        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (m_gameManager == null)
        {
            Debug.Log("m_gameManager not found!");
        }

        if (m_speedText == null)
        {
            Debug.Log("m_speedText not assigned!");
        }

        if (m_timeText == null)
        {
            Debug.Log("m_timeText not assigned!");
        }

        if (m_speedGauge == null)
        {
            Debug.Log("m_speedGauge not assigned!");
        }

        if (m_excessSpeedGauge == null)
        {
            Debug.Log("m_excessSpeedGauge not assigned!");
        }

        if (m_dashGauge == null)
        {
            Debug.Log("m_speedGauge not assigned!");
        }

        m_player = GameObject.FindGameObjectWithTag("Player");

        if (m_player == null)
        {
            Debug.Log("m_player not found!");
        }

        m_playerController = m_player.GetComponent<PlayerController>();
        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_dashGauge.fillAmount = m_playerController.GetAirDashes() / 3.0f;

        float speed = m_playerController.GetSpeed() / m_playerController.GetMaxSpeed();
        m_speedGauge.fillAmount = Mathf.Min(1.0f, speed);
        m_speedGauge.color = Color.green * (1.0f - speed) + Color.yellow * speed;
        m_speedGauge.color = new Color(m_speedGauge.color.r, m_speedGauge.color.g, m_speedGauge.color.b, 0.58f);

        m_excessSpeedGauge.fillAmount = Mathf.Min(1.0f, speed - 1.0f);

        float mph = Mathf.Abs(m_playerController.GetSpeed()) * 2.23694f;
        m_speedText.text = string.Format("{0:n0}mph", mph);

        TimeSpan t = TimeSpan.FromSeconds(m_gameManager.GetTimeElapsed());
        m_timeText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
    }
}
