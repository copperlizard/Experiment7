using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    private GameManager m_gameManager;

	// Use this for initialization
	void Start ()
    {
        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (m_gameManager == null)
        {
            Debug.Log("m_gameManager not found!");
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
            Debug.Log("GOOOOOOOOOAAAAALLLLLLLL!!!!!!!!");

            m_gameManager.LevelFinished();
        }
    }
}
