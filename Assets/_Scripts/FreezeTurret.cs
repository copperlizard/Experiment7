using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTurret : MonoBehaviour
{
    [SerializeField]
    private float m_turretRotationSpeed = 15.0f;

    private GameObject m_player;

    private BasicRotator m_turretRotator;
    
    private bool m_playerDetected = false, m_lockOn = false, m_firing = false;

	// Use this for initialization
	void Start ()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        if (m_player == null)
        {
            Debug.Log("m_player not found!");
        }

        m_turretRotator = GetComponentInChildren<BasicRotator>();
        if (m_turretRotator == null)
        {
            Debug.Log("m_turretRotator not found!");
        }        
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (m_playerDetected && !m_lockOn) //Look at player
        {
            FacePlayer();
        }
        else if (m_lockOn && !m_firing) //Lock on and spin up turret
        {
            LockOn();
        }
        else if (m_firing) //Fire turret
        {
            Fire();
        }
        else if (!m_firing && m_turretRotator.GetRotation().z > 0.0f) //Spin down turret
        {

        }
	}

    private void FacePlayer ()
    {
        Vector3 toPlayer = (m_player.transform.position + m_player.transform.up * 1.3f) - transform.position;

        if (Vector3.Dot(transform.forward, toPlayer.normalized) > 0.85f)
        {
            m_lockOn = true;
            return;
        }

        Quaternion tarRot = Quaternion.LookRotation(toPlayer.normalized);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, tarRot, 5.0f);
    }

    private void LockOn ()
    {
        Vector3 toPlayer = (m_player.transform.position + m_player.transform.up * 1.3f) - transform.position;
        
        transform.rotation = Quaternion.LookRotation(toPlayer.normalized);

        float z = m_turretRotator.GetRotation().z;

        if (z == m_turretRotationSpeed)
        {
            m_firing = true;
            return;
        }

        if (z >= m_turretRotationSpeed - 0.01f)
        {
            m_turretRotator.SetRotation(new Vector3(0.0f, 0.0f, m_turretRotationSpeed));
        }
        else 
        {
            m_turretRotator.SetRotation(new Vector3(0.0f, 0.0f, Mathf.Lerp(z, m_turretRotationSpeed, 3.0f * Time.deltaTime)));
        }
    }

    private void Fire ()
    {
        Vector3 toPlayer = (m_player.transform.position + m_player.transform.up * 1.3f) - transform.position;
        
        transform.rotation = Quaternion.LookRotation(toPlayer.normalized);
    }

    private void OnTriggerStay(Collider other)
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
            m_lockOn = false; 
            m_firing = false;
        }
    }
}
