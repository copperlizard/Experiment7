using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBox : MonoBehaviour
{
    [SerializeField]
    private float m_bounceForce;

    [SerializeField]
    private Vector3 m_bounceAxis;

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
        if (collision.rigidbody != null && collision.gameObject.tag != "Player")
        {
            collision.rigidbody.velocity += collision.contacts[0].normal * m_bounceForce;

            //StartCoroutine(AnimateBounce());
        }
        else if (collision.gameObject.tag == "Player")
        {
            m_playerController.Bounce(collision.contacts[0].normal, m_bounceForce);
        }
    }

    private IEnumerator AnimateBounce ()
    {


        yield return null;
    }
}
