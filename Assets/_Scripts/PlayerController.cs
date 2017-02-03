using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody m_playerRB;

    private Vector3 m_playerVelocity;

	// Use this for initialization
	void Start ()
    {
        m_playerRB = GetComponent<Rigidbody>();

        if (m_playerRB == null)
        {
            Debug.Log("m_playerRB not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public void Move (Vector2 move)
    {
        m_playerVelocity = Vector3.Lerp(m_playerVelocity, new Vector3(move.x, 0.0f, move.y), 0.5f);
        m_playerRB.velocity = m_playerVelocity;

        if (move.magnitude < 0.05)
        {
            return;
        }
        else
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(move.x, 0.0f, move.y));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.5f);
        }
    } 

    public void Jump ()
    {

    }
}
