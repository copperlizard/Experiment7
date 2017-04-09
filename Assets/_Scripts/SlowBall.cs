using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBall : Destructible
{
    [SerializeField]
    private float m_followSpeed = 5.0f, m_maxFollowDist = 300.0f;

    [SerializeField]
    private AudioClip m_destructionSound;

    [SerializeField]
    private Light m_light;

    private SlowBallTriggerZone m_triggerZone;

    private GameObject m_player, m_sphere, m_destruction;
    private PlayerController m_playerController;

    private Rigidbody m_ballRB;

    private AudioSource m_ballAudioSource;

    private Vector3 m_startLocalPos;

    private float m_amountSlowed = 0.0f;

    private bool m_playerDetected = false, m_stuck = false, m_destructed = false, m_smoking = false, m_slowing = false;

    public override void Destruct()
    {
        if (m_destructed || m_smoking)
        {
            return;
        }

        m_destructed = true;

        if (m_stuck && m_slowing)
        {
            m_slowing = false;
            m_playerController.AdjustSpeedMod(m_amountSlowed);
        }        

        m_sphere.SetActive(false);

        m_destruction.SetActive(true);
        
        StartCoroutine(WaitForSmoke());        
    }

    private IEnumerator WaitForSmoke ()
    {
        m_smoking = true;

        m_ballAudioSource.PlayOneShot(m_destructionSound, 0.1f);
        yield return new WaitForSeconds(0.25f);
        m_destruction.SetActive(false);    
        
        if (m_triggerZone != null) //Not recycled
        {
            Destroy(gameObject);
            yield break;
        }    
        
        m_destructed = false;
        m_stuck = false;
        
        if (m_ballRB == null)
        {
            m_ballRB = gameObject.AddComponent<Rigidbody>();
            m_ballRB.useGravity = false;
            m_ballRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        transform.parent = null;
        m_smoking = false;

        m_sphere.SetActive(true);
        gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start ()
    {
        if (m_destructionSound == null)
        {
            Debug.Log("m_destructionSound not assigned!");
        }

        if (m_light == null)
        {
            Debug.Log("m_light not assigned!");
        }

        m_sphere = transform.GetChild(0).gameObject;
        if (m_sphere == null)
        {
            Debug.Log("m_sphere not found!");
        }

        m_destruction = transform.GetChild(1).gameObject;
        if (m_destruction == null)
        {
            Debug.Log("m_destruction not found!");
        }
        
        m_ballAudioSource = GetComponent<AudioSource>();
        if (m_ballAudioSource == null)
        {
            Debug.Log("m_ballAudioSource not found!");
        }

        m_ballRB = GetComponent<Rigidbody>();
        if (m_ballRB == null)
        {
            Debug.Log("m_playerRB not found!");            
        }

        m_triggerZone = GetComponentInParent<SlowBallTriggerZone>();
        if (m_triggerZone == null)
        {
            Debug.Log("m_triggerZone not assigned or found!");
            m_playerDetected = true;

            m_player = GameObject.FindGameObjectWithTag("Player");
            m_playerController = m_player.GetComponent<PlayerController>();

            m_ballAudioSource.Play();
            m_light.enabled = true;
        }
        else
        {
            m_player = m_triggerZone.GetPlayer();
            m_playerController = m_triggerZone.GetPlayerController();
        }

        if (m_player == null)
        {
            Debug.Log("m_player not found!");
        }

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
            /*if (m_ballRB == null) //REALLY SHOULDN'T BE NECCESSARY
            {
                m_ballRB = gameObject.AddComponent<Rigidbody>();
                m_ballRB.useGravity = false;
                m_ballRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }*/

            if (!m_playerDetected)
            {
                if (m_triggerZone != null)
                {
                    m_playerDetected = m_triggerZone.PlayerDetected();
                }                

                if (m_playerDetected)
                {
                    if (m_triggerZone != null)
                    {
                        m_ballRB.velocity = m_triggerZone.transform.TransformVector(m_startLocalPos.normalized * m_followSpeed);
                    }                                        
                    
                    transform.parent = null;
                    m_ballAudioSource.Play();
                    m_light.enabled = true;
                }
                else
                {
                    //transform.localPosition = m_startLocalPos; //ensure no drift while trigger zone spins
                }
            }
            else
            {
                if (!m_light.enabled) //Recycled bossballs need this...
                {
                    m_ballAudioSource.Play();
                    m_light.enabled = true;
                }

                ChasePlayer();
            }
        }         
	}

    private void ChasePlayer ()
    {
        if (m_ballRB == null)
        {
            return;
        }

        float now = Time.time + GetInstanceID() * 50.0f;
        float h = Mathf.Sin(now) * 0.5f + 0.5f;
        m_light.color = Color.red * h + Color.blue * (1.0f - h);
        
        Vector3 toPlayer = (m_player.transform.position + m_player.transform.up * (1.6f - h)) - transform.position;

        if (toPlayer.magnitude > m_maxFollowDist)
        {
            m_ballRB.velocity = Vector3.zero;
            m_playerDetected = false;
            m_ballAudioSource.Stop();
            m_light.enabled = false;
        }
        else
        {
            m_ballRB.velocity = Vector3.Lerp(m_ballRB.velocity, toPlayer.normalized * m_followSpeed, 1.5f * Time.deltaTime);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log(gameObject.name + " collided with " + collision.collider.gameObject.name);

        if (collision.collider.tag == "PlayerBody")
        {
            //Debug.Log("stuck to " + collision.collider.gameObject.name);

            if (!m_stuck)
            {
                m_ballAudioSource.Stop();
                m_stuck = true;
                m_light.enabled = false;

                if (!m_slowing)
                {
                    m_slowing = true;

                    m_amountSlowed = Mathf.Min(0.05f, m_playerController.GetSpeedMod());

                    m_playerController.AdjustSpeedMod(-m_amountSlowed);
                }
                
                transform.parent = collision.collider.gameObject.transform;

                m_ballRB.velocity = Vector3.zero;
                Destroy(m_ballRB);
                
                transform.position += -collision.contacts[0].normal * (transform.localScale.x * 0.5f);
            }                         
        }
    }

    public bool IsStuck ()
    {
        return m_stuck;
    }
}
