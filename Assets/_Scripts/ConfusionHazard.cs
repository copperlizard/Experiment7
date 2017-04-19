using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionHazard : MonoBehaviour
{
    private CameraVortexManager m_camVortexManager;

    private bool m_playerPresent = false;

	// Use this for initialization
	void Start ()
    {
        m_camVortexManager = Camera.main.GetComponent<CameraVortexManager>();
        if (m_camVortexManager == null)
        {
            Debug.Log("m_camVortexManager not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (m_playerPresent)
        {
            m_camVortexManager.AddTime(Time.deltaTime * 50.0f);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_playerPresent = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_playerPresent = false;
        }
    }
}
