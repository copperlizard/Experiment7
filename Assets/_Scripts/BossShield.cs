using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : MonoBehaviour
{
    [SerializeField]
    private float m_pushForce = 3.0f;

    private PlayerController m_playerController;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            Vector3 dir = collision.contacts[0].point - transform.position;
            dir = dir.normalized;

            Vector3 fudge = Vector3.ProjectOnPlane(dir, transform.up).normalized;

            dir = (dir + fudge).normalized;
            
            collision.rigidbody.velocity += dir * m_pushForce;            
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            Vector3 dir = collision.contacts[0].point - transform.position;
            dir = dir.normalized;

            Vector3 fudge = Vector3.ProjectOnPlane(dir, transform.up).normalized;

            dir = (dir + fudge).normalized;

            collision.rigidbody.velocity += dir * m_pushForce;
        }
    }
}
