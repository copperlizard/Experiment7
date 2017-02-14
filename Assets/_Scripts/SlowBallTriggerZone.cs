using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBallTriggerZone : MonoBehaviour
{
    private GameObject m_player;
    private PlayerController m_playerController;

    private bool m_playerDetected = false;

	// Use this for initialization
	void Awake ()
    {
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
		if (!m_playerDetected)
        {
            transform.rotation *= Quaternion.Euler(150.0f * Mathf.Cos(Time.time) * Time.deltaTime, 290.0f * Time.deltaTime, 220.0f * Time.deltaTime);
        }
	}

    public GameObject GetPlayer ()
    {
        return m_player;
    }

    public PlayerController GetPlayerController ()
    {
        return m_playerController;
    }

    public bool PlayerDetected ()
    {
        return m_playerDetected;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_playerDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_playerDetected = false;
        }
    }
}
