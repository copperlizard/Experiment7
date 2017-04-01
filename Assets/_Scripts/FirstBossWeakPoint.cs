using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossWeakPoint : MonoBehaviour
{
    private FirstBossController m_bossController;

	// Use this for initialization
	void Start ()
    {
        m_bossController = GetComponentInParent<FirstBossController>();
        if (m_bossController == null)
        {
            Debug.Log("m_bossContoller not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_bossController.HitWeakPoint(collision.contacts[0].normal);
        }
    }
}
