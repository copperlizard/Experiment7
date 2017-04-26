using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneIndicator : MonoBehaviour
{
    private GameObject m_player;

	// Use this for initialization
	void Start ()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        if (m_player == null)
        {
            Debug.Log("m_player not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
	}
}
