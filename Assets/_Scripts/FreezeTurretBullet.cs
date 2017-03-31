using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTurretBullet : Destructible
{
    [SerializeField]
    private GameObject m_bullet, m_destruction;    

    [SerializeField]
    private float m_maxFlightTime = 10.0f;

    private Rigidbody m_bulletRB;

    private FreezeTurret m_turret = null;

    private float m_firedTime = 0.0f;

    private bool m_destructed = false, m_fired = false;

    public override void Destruct ()
    {
        if (m_destructed)
        {
            return;
        }

        m_bullet.SetActive(false);
        m_destruction.SetActive(true);

        SprayIce();

        m_destructed = true;

        StartCoroutine(DeactivateSelf());        
    }

    private IEnumerator DeactivateSelf ()
    {
        yield return new WaitForSeconds(0.25f); //wait for destruction FX
        m_destruction.SetActive(false);
        m_bullet.SetActive(true);
        m_destructed = false;
        m_fired = false;
        m_bulletRB.isKinematic = false;
        m_bulletRB.detectCollisions = true;

        gameObject.SetActive(false);
        yield return null;
    }

    // Use this for initialization
    void Awake ()
    {
        m_bulletRB = GetComponent<Rigidbody>();
        if (m_bulletRB == null)
        {
            Debug.Log("m_bulletRB not found!");
        }

		if (m_bullet == null)
        {
            Debug.Log("m_bullet not assigned!");
        }

        if (m_destruction == null)
        {
            Debug.Log("m_destruction not assigned!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (m_fired && Time.time > m_firedTime + m_maxFlightTime && !m_destructed)
        {
            Destruct();
        }
	}

    private void SprayIce ()
    {
        m_turret.SprayIce(transform);
    }

    public void SetTurret(FreezeTurret turret)
    {
        m_turret = turret;
    }

    public void Fire(Quaternion rot, float speed)
    {
        m_destruction.SetActive(false);
        m_bullet.SetActive(true);

        m_destructed = false;
        m_fired = false;
        
        m_bulletRB.isKinematic = false;
        m_bulletRB.detectCollisions = true;

        transform.rotation = rot;
        m_bulletRB.velocity = transform.forward * speed;

        m_firedTime = Time.time;
        m_fired = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("bullet hit " + collision.gameObject.name);

        if (m_fired)
        {
            m_bulletRB.velocity = Vector3.zero;
            m_bulletRB.isKinematic = true;
            m_bulletRB.detectCollisions = false;

            if (!m_destructed)
            {
                Destruct();
            }
        }       
    }
}
