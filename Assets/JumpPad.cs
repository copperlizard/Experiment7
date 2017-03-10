using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private PlayerController m_playerController;

    private bool m_playerDetected = false;

	// Use this for initialization
	void Start ()
    {
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!m_playerDetected)
            {
                m_playerDetected = true;
                m_playerController.SetJumpPad(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (m_playerDetected)
            {
                m_playerDetected = false;
                m_playerController.SetJumpPad(false);
            }
        }
    }
}
