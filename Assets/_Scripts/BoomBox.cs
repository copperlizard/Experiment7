using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBox : MonoBehaviour
{
    [SerializeField]
    float m_minProximity = 5.0f, m_boxSpeed = 15.0f, m_explosionForce = 50.0f, m_explosionRadius = 10.0f;

    private GameObject m_player;

    private PlayerController m_playerController;

    private Rigidbody m_boxRB;

    private float m_proximity = 100.0f;

    private bool m_playerDetected = false, m_detonated = false;

	// Use this for initialization
	void Start ()
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

        m_boxRB = GetComponent<Rigidbody>();
        if (m_boxRB == null)
        {
            Debug.Log("m_boxRB not found!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (m_playerDetected)
        {
            CheckProximity();
        }

        if (!m_detonated)
        {
            transform.Rotate(0.0f, (600.0f * Mathf.SmoothStep(0.0f, 1.0f, (m_minProximity / m_proximity))) * Time.deltaTime, 0.0f);
        }        
	}

    private void CheckProximity ()
    {
        Vector3 toPlayer = m_player.transform.position - transform.position;
        m_proximity = toPlayer.magnitude;

        if (m_proximity < m_minProximity)
        {
            m_boxRB.velocity = Vector3.Lerp(m_boxRB.velocity, -transform.up * m_boxSpeed, 5.0f * Time.deltaTime);
        }
    }

    private void Detonate ()
    {
        m_boxRB.velocity = Vector3.zero;

        if (m_detonated)
        {
            return;
        }

        m_detonated = true;

        Debug.Log("Boom!");

        //Visualize explosion

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_explosionRadius, ~LayerMask.GetMask("Default", "PlayerBody"));

        for (int i = 0; i < hitColliders.Length; i++)
        {   
            Debug.Log("explosion hit " + hitColliders[i].gameObject.name);

            if (hitColliders[i].tag == "Player")
            {
                m_playerController.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0.5f);

                Debug.Log("exploding Player!");
            }
            else
            {
                Rigidbody hitRB = hitColliders[i].attachedRigidbody;

                if (hitRB != null)
                {
                    if (hitColliders[i].transform.parent != null)
                    {
                        //object has parent                        
                        if (hitColliders[i].transform.parent.gameObject.layer != LayerMask.NameToLayer("PlayerBody"))
                        {
                            hitRB.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0.5f);
                        }
                    }
                    else
                    {
                        hitRB.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0.5f);
                    }
                }
            }            
        }

        //Devisualize explosion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_playerDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_playerDetected = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Detonate();
    }
}
