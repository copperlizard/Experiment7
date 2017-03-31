using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCannonBall : Destructible
{
    [SerializeField]
    private GameObject m_ball, m_destruction;

    [SerializeField]
    private float m_explosionForce = 10.0f, m_explosionRadius = 10.0f;

    private Rigidbody m_ballRB;

    private PlayerController m_playerController;

    private bool m_destructed = false;

    public override void Destruct()
    {
        if (m_destructed)
        {
            return;
        }

        m_ball.SetActive(false);
        m_destruction.SetActive(true);
                
        m_destructed = true;

        ExplodeStuff();

        StartCoroutine(DeactivateSelf());
    }

    private IEnumerator DeactivateSelf()
    {
        yield return new WaitForSeconds(0.25f); //wait for destruction FX
        m_destruction.SetActive(false);
        m_ball.SetActive(true);
        m_destructed = false;        
        m_ballRB.isKinematic = false;
        m_ballRB.detectCollisions = true;

        gameObject.SetActive(false);
        yield return null;
    }

    // Use this for initialization
    void Start ()
    {
        m_ballRB = GetComponent<Rigidbody>();        
        if (m_ballRB == null)
        {
            Debug.Log("m_ballRB not found!");
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

    private void ExplodeStuff ()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_explosionRadius, ~LayerMask.GetMask("PlayerBody"), QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitColliders.Length; i++)  // CHECK FOR STUCK SLOWBALLS AND MOVE TO DEFAULT LAYER!!!
        {
            //Debug.Log("ice overlap with " + hitColliders[i].name);

            if (hitColliders[i].tag == "Player")
            {
                m_playerController.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0.5f);
            }
            else if (hitColliders[i].attachedRigidbody != null)
            {
                hitColliders[i].attachedRigidbody.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0.5f, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_ballRB.velocity = Vector3.zero;
        m_ballRB.isKinematic = true;
        m_ballRB.detectCollisions = false;

        if (!m_destructed)
        {
            Destruct();
        }        
    }
}
