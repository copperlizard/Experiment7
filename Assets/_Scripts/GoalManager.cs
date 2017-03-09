using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    private GameManager m_gameManager;

    private PlayerController m_playerController;

    private Rigidbody m_playerRB;

    private bool m_finished = false;

	// Use this for initialization
	void Start ()
    {
        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (m_gameManager == null)
        {
            Debug.Log("m_gameManager not found!");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_playerController = player.GetComponent<PlayerController>();
        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }
        m_playerRB = player.GetComponent<Rigidbody>();
        if (m_playerRB == null)
        {
            Debug.Log("m_playerRB not found!");
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
            if (!m_finished)
            {
                m_finished = true;

                //Debug.Log("goaaal!");

                m_playerController.FreeFly();

                m_playerRB.transform.rotation = Quaternion.LookRotation(-transform.forward);

                m_playerRB.velocity += -transform.forward * 3000.0f;

                m_gameManager.LevelFinished();
            }            
        }
    }
}
