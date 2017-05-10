using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPickup : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pickup, m_pickedUp;

    [SerializeField]
    private float m_stamina = 0.5f;

    private PlayerController m_playerController;
    
	// Use this for initialization
	void Start ()
    {
	    if (m_pickup == null)
        {
            Debug.Log("m_pickup not assigned!");
        }

        if (m_pickedUp == null)
        {
            Debug.Log("m_pickedUp not assigned!");
        }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_pickup.SetActive(false);
            m_pickedUp.SetActive(true);
            m_playerController.SetAirDashes(m_playerController.GetAirDashes() + m_stamina);
            StartCoroutine(CleanUpPickUp());
        }
    }

    IEnumerator CleanUpPickUp ()
    {
        yield return new WaitForSeconds(1.0f);
        m_pickedUp.SetActive(false);
        m_pickup.SetActive(true);
        gameObject.SetActive(false);
        yield return null;
    }
}
