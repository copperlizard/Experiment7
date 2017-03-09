using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpPickUp : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pill;

    [SerializeField]
    private AudioClip m_pickUpSound;
    
    [SerializeField]
    private Vector3 m_rotateEuler = new Vector3(5.0f, 15.0f, 33.0f);

    [SerializeField]
    private float m_speedUp = 0.1f;

    private AudioSource m_audioSource;

    private PlayerController m_playerController;

    private bool m_pickedUp = false;

	// Use this for initialization
	void Start ()
    {
        if (m_pill == null)
        {
            Debug.Log("m_pill not assigned!");
        }


        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }

        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            Debug.Log("m_audioSource not found!");
        }

        if (m_pickUpSound == null)
        {
            Debug.Log("m_pickUpSound not assigned!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(m_rotateEuler * Time.deltaTime);
	}

    private void PickedUp ()
    {
        if (m_pickedUp)
        {
            return;
        }

        m_pickedUp = true;

        //Debug.Log("speed up!");

        m_playerController.AdjustSpeedMod(m_speedUp);
                
             
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear ()
    {
        m_pill.SetActive(false);
        m_audioSource.Stop(); // stop waiting sound
        m_audioSource.PlayOneShot(m_pickUpSound); // play pick up sound once

        do
        {
            yield return null;
        } while (m_audioSource.isPlaying);
        Destroy(gameObject);
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {            
            PickedUp();
        }
    }
}
