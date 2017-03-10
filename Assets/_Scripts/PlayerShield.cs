using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [SerializeField]
    private bool m_dashRegen = false;

    [SerializeField]
    private float m_dashRegenFactor = 1.5f;

    private PlayerController m_playerController;

    private AudioSource m_audioSource;

	// Use this for initialization
	void Start ()
    {
        m_playerController = GetComponentInParent<PlayerController>();
        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }

        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            Debug.Log("m_audioSource not found!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_audioSource.pitch = Mathf.Lerp(m_audioSource.pitch, Random.Range(0.85f, 1.15f), 3.0f * Time.deltaTime);

		if (transform.localScale.x < 0.95f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.localScale.x + 0.1f, LayerMask.GetMask("Projectile"));

            for (int i = 0; i < hitColliders.Length; i++)
            {
                Destructible destroyable = (Destructible)hitColliders[i].gameObject.GetComponent<SlowBall>();

                if (destroyable != null)
                {
                    if (m_dashRegen)
                    {
                        m_playerController.SetAirDashes(m_playerController.GetAirDashes() + m_dashRegenFactor);
                    }
                    
                    destroyable.Destruct();
                }
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        Destructible destroyable = (Destructible)other.GetComponent<SlowBall>();

        if (destroyable != null)
        {
            destroyable.Destruct();
        }
    }
}
