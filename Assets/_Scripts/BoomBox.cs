using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBox : MonoBehaviour
{
    [SerializeField]
    private GameObject m_box, m_explosion;

    [SerializeField]
    float m_minProximity = 5.0f, m_boxSpeed = 15.0f, m_explosionForce = 50.0f, m_explosionRadius = 10.0f, m_explosionDuration = 0.5f;

    private GameObject m_player;

    private PlayerController m_playerController;

    private AudioSource m_beeperAudioSource;

    private Rigidbody m_boxRB;

    private Light m_boxLight;

    private float m_proximity = 100.0f, m_flashTimer = 0.0f, m_flashPeriod = 3.0f;

    private bool m_playerDetected = false, m_detonated = false;

	// Use this for initialization
	void Start ()
    {
        if (m_box == null)
        {
            Debug.Log("m_box not assigned!");
        }

        m_boxLight = m_box.GetComponentInChildren<Light>();
        if (m_boxLight == null)
        {
            Debug.Log("m_boxLight not found!");
        }

        if (m_explosion == null)
        {
            Debug.Log("m_box not assigned!");
        }

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

        m_beeperAudioSource = GetComponent<AudioSource>();
        if (m_beeperAudioSource == null)
        {
            Debug.Log("m_beeperAudioSource not found!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (m_playerDetected && !m_detonated)
        {
            CheckProximity();
        }
        else
        {
            m_proximity = 100.0f;

            if (m_boxLight.enabled)
            {
                m_boxLight.enabled = false;
            }
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

        m_flashPeriod = 3.0f * Mathf.Lerp(1.0f, 0.1f, (m_minProximity / m_proximity));

        if (Time.time > m_flashTimer + m_flashPeriod)
        {
            //Debug.Log("beep!");

            m_flashTimer = Time.time;

            if (!m_beeperAudioSource.isPlaying && m_playerDetected)
            {
                m_beeperAudioSource.PlayOneShot(m_beeperAudioSource.clip);
            }
        }

        if (m_beeperAudioSource.isPlaying && !m_boxLight.enabled)
        {
            m_boxLight.enabled = true;
        }
        else if (!m_beeperAudioSource.isPlaying && m_boxLight.enabled)
        {
            m_boxLight.enabled = false;
        }
    }

    private void Detonate ()
    {
        m_boxRB.velocity = Vector3.zero;
        m_boxRB.isKinematic = true;

        if (m_detonated)
        {
            return;
        }

        m_detonated = true;

        //Debug.Log("Boom!");

        //Visualize explosion
        m_box.SetActive(false);
        m_explosion.SetActive(true);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_explosionRadius, ~LayerMask.GetMask("Default", "PlayerBody"));

        for (int i = 0; i < hitColliders.Length; i++)
        {   
            //Debug.Log("explosion hit " + hitColliders[i].gameObject.name);

            if (hitColliders[i].tag == "Player")
            {
                m_playerController.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0.5f);

                //Debug.Log("exploding Player!");
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

        StartCoroutine(WaitForExplosion());
    }

    private IEnumerator WaitForExplosion ()
    {
        yield return new WaitForSeconds(m_explosionDuration);
        //Devisualize explosion
        m_explosion.SetActive(false);
        yield return null;
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
