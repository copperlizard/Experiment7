using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBall : MonoBehaviour
{
    [SerializeField]
    private float m_followSpeed = 5.0f, m_maxFollowDist = 300.0f;

    private SlowBallTriggerZone m_triggerZone;

    private GameObject m_player;
    private PlayerController m_playerController;

    private Rigidbody m_ballRB;

    private Vector3 m_startLocalPos;

    private bool m_playerDetected = false, m_stuck = false;

	// Use this for initialization
	void Start ()
    {
        m_triggerZone = GetComponentInParent<SlowBallTriggerZone>();

        if (m_triggerZone == null)
        {
            Debug.Log("m_triggerZone not found!");
        }

        m_player = m_triggerZone.GetPlayer();
        if (m_player == null)
        {
            Debug.Log("m_player not found!");
        }

        m_ballRB = GetComponent<Rigidbody>();
        if (m_ballRB == null)
        {
            Debug.Log("m_playerRB not found!");
        }

        m_playerController = m_player.GetComponent<PlayerController>();
        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }

        m_startLocalPos = transform.localPosition;
    }
	
	// Update is called once per frame
	void Update ()
    {   
        if (!m_stuck)
        {
            if (!m_playerDetected)
            {
                m_playerDetected = m_triggerZone.PlayerDetected();

                if (m_playerDetected)
                {                    
                    m_ballRB.velocity = transform.parent.TransformVector(m_startLocalPos.normalized * m_followSpeed);
                    transform.parent = null;
                }
                else
                {
                    transform.localPosition = m_startLocalPos; //ensure no drift while trigger zone spins
                }
            }
            else
            {
                SeekPlayer();
            }
        }             
	}

    private void SeekPlayer ()
    {
        Vector3 toPlayer = (m_player.transform.position + m_player.transform.up * 1.6f) - transform.position;

        if (toPlayer.magnitude > m_maxFollowDist)
        {
            m_ballRB.velocity = Vector3.zero;
            m_playerDetected = false;
        }
        else
        {
            m_ballRB.velocity = Vector3.Lerp(m_ballRB.velocity, toPlayer.normalized * m_followSpeed, 1.5f * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_stuck = true;
            m_ballRB.velocity = Vector3.zero;
            m_ballRB.mass = 0.0f;
            transform.parent = collision.transform;
        }
    }
}
