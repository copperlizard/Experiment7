using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBox : MonoBehaviour
{
    [SerializeField]
    private GameObject m_bounceFX;

    [SerializeField]
    private float m_bounceForce, m_bounceCooldown = 0.25f;

    [SerializeField]
    private Vector3 m_bounceAxis;

    private PlayerController m_playerController;

    private bool m_playerBounced = false;

	// Use this for initialization
	void Start ()
    {
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }

        if (m_bounceFX == null)
        {
            Debug.Log("m_bounceFX not assigned!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !m_playerBounced)
        {
            m_playerController.Bounce(Vector3.up, m_bounceForce);
            m_playerBounced = true;

            StartCoroutine(AnimateBounce());
        }
        else if (other.tag != "Player" && other.attachedRigidbody != null)
        {
            StartCoroutine(AnimateBounce());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && m_playerBounced)
        {
            StartCoroutine(BounceCooldown());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && collision.gameObject.tag != "Player")
        {
            //collision.rigidbody.velocity += collision.contacts[0].normal * m_bounceForce;

            StartCoroutine(AnimateBounce());
        }
        else if (collision.gameObject.tag == "Player" && !m_playerBounced)
        {
            m_playerController.Bounce(collision.contacts[0].normal, m_bounceForce);
            m_playerBounced = true;

            StartCoroutine(AnimateBounce());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && m_playerBounced)
        {
            StartCoroutine(BounceCooldown());
        }
    }

    private IEnumerator BounceCooldown ()
    {
        yield return new WaitForSeconds(m_bounceCooldown);
        m_playerBounced = false;
    }

    private IEnumerator AnimateBounce ()
    {
        if (m_bounceFX != null)
        {
            if (m_bounceFX.activeInHierarchy)
            {
                m_bounceFX.SetActive(false); // reset FX
            }

            m_bounceFX.SetActive(true);
            
            yield return new WaitForSeconds(1.0f);

            if (m_bounceFX.activeInHierarchy) // avoid confusion with multiple collsions
            {
                m_bounceFX.SetActive(false); // reset FX
            }
        }

        yield return null;
    }
}
