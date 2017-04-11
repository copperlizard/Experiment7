using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player, m_leftArm, m_rightArm, m_leftTarget, 
        m_rightTarget, m_shield, m_slowBallSpawnPointsParent;

    [SerializeField]
    private float m_maxTurnSpeed = 15.0f, m_maxAimSpeed = 15.0f;

    private PlayerController m_playerController;

    private LineRenderer m_leftLazer, m_rightLazer;

    private ObjectPool m_cannonAmmo, m_slowBalls;

    private AudioSource m_audioSource;

    private Transform[] m_slowBallSpawnPoints;

    private Vector3 m_tarPos;

    private int m_hits = 0;

    private bool m_leftArmLocked = false, m_rightArmLocked = false, m_firing = false, m_leftArmFiring = false, 
        m_rightArmFiring = false, m_weakPointHit = false, m_shielding = false;

    // Use this for initialization
    void Start ()
    {        
        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            Debug.Log("m_audioSource not found!");
        }

		if (m_player == null)
        {
            m_player = GameObject.FindGameObjectWithTag("Player");

            if (m_player == null)
            {
                Debug.Log("m_player not assigned or found!");
            }
        }

        m_playerController = m_player.GetComponent<PlayerController>();
        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }

        if (m_slowBallSpawnPointsParent == null)
        {
            Debug.Log("m_slowBallSpawnPointsParent not assigned!");
        }
        else
        {
            m_slowBallSpawnPoints = m_slowBallSpawnPointsParent.GetComponentsInChildren<Transform>();
        }

        if (m_leftArm == null)
        {
            Debug.Log("m_leftArm not assigned!");
        }
        else
        {
            m_leftLazer = m_leftArm.GetComponent<LineRenderer>();
        }

        if (m_rightArm == null)
        {
            Debug.Log("m_rightArm not assigned!");
        }
        else
        {
            m_rightLazer = m_rightArm.GetComponent<LineRenderer>();
        }

        if (m_leftLazer == null)
        {
            Debug.Log("m_leftLazer not found!");
        }

        if (m_rightLazer == null)
        {
            Debug.Log("m_rightLazer not found!");
        }

        if (m_leftTarget == null)
        {
            Debug.Log("m_leftTarget not assigned!");
        }

        if (m_rightTarget == null)
        {
            Debug.Log("m_rightTarget not assigned!");
        }

        m_cannonAmmo = GetComponentsInChildren<ObjectPool>()[0];
        if (m_cannonAmmo == null)
        {
            Debug.Log("m_cannonAmmo not found!");
        }

        m_slowBalls = GetComponentsInChildren<ObjectPool>()[1];
        if (m_slowBalls == null)
        {
            Debug.Log("m_slowBalls not found!");
        }

        m_leftTarget.transform.parent = null;
        m_rightTarget.transform.parent = null;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.timeScale > 0.0f && !m_weakPointHit)
        {
            FacePlayer();

            AimGunArm(m_leftArm);
            AimGunArm(m_rightArm);
        }        
    }

    void BossShield ()
    {
        if (!m_shielding)
        {
            m_shielding = true;
            StartCoroutine(BossShielding());
        }
    }

    IEnumerator BossShielding ()
    {
        m_shield.SetActive(true);

        foreach (Transform spawnpoint in m_slowBallSpawnPoints)
        {
            GameObject slowBall = m_slowBalls.GetObject();
            slowBall.transform.position = spawnpoint.position;
            slowBall.SetActive(true);

            Rigidbody slowBallRB = slowBall.GetComponent<Rigidbody>();
            slowBallRB.velocity += Vector3.up * 20.0f;
        }

        do
        {
            float s = Mathf.Min(1.0f, m_shield.transform.localScale.x + 3.0f * Time.deltaTime);
            m_shield.transform.localScale = Vector3.one * s;

            yield return null;
        } while (m_shield.transform.localScale.x < 1.0f);

        yield return new WaitForSeconds(15.0f);

        m_shield.transform.localScale = Vector3.zero;
        m_shield.SetActive(false);

        m_shielding = false;
        m_weakPointHit = false;

        yield return null;
    }

    void BossDie ()
    {
        StartCoroutine(BossDieing());
    }

    IEnumerator BossDieing ()
    {
        float startTime = Time.time;
        do
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - transform.up, 10.0f * Time.deltaTime);
            transform.Rotate(0.0f, 20.0f * Time.deltaTime, 0.0f);
            yield return null;
        } while (Time.time < startTime + 10.0f);

        Destroy(gameObject);
    }

    void FacePlayer ()
    {
        Vector3 toPlayer = m_player.transform.position - transform.position;

        toPlayer = Vector3.ProjectOnPlane(toPlayer, transform.up);

        Quaternion tarRot = Quaternion.LookRotation(toPlayer.normalized);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, tarRot, m_maxTurnSpeed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, m_maxTurnSpeed * Time.deltaTime);
    }

    void FireGunArm (GameObject arm)
    {
        if (arm == m_leftArm)
        {
            if (!m_leftArmFiring)
            {
                //Debug.Log("left arm fire!");

                m_leftArmFiring = true;
                StartCoroutine(GunArmFire(arm));
            }
        }
        else if (arm == m_rightArm)
        {
            if (!m_rightArmFiring)
            {
                //Debug.Log("right arm fire!");

                m_rightArmFiring = true;
                StartCoroutine(GunArmFire(arm));
            }
        }
    }

    IEnumerator GunArmFire (GameObject arm)
    {
        m_firing = true;

        Vector3 tarPos;
        RaycastHit hit;

        float startTime = Time.time, beepTime = 0.0f, beepPeriod = 0.75f;

        do
        {
            if (Physics.Raycast(arm.transform.position, arm.transform.forward, out hit, 1000.0f, ~LayerMask.GetMask("Player", "Boss", "Projectile"), QueryTriggerInteraction.Ignore))
            {
                tarPos = hit.point;

                //Debug.Log("hit " + hit.collider.gameObject.name);
            }
            else
            {
                tarPos = arm.transform.position + arm.transform.forward * 1000.0f;
            }            

            if (arm == m_leftArm)
            {
                //Visualize aim

                m_leftLazer.SetPosition(0, arm.transform.position);
                m_leftLazer.SetPosition(1, tarPos);

                if (!m_leftTarget.activeInHierarchy)
                {
                    m_leftTarget.SetActive(true);
                }

                m_leftTarget.transform.position = tarPos;
                m_leftTarget.transform.up = hit.normal;
            }
            else if (arm == m_rightArm)
            {
                //Visualize aim

                m_rightLazer.SetPosition(0, arm.transform.position);
                m_rightLazer.SetPosition(1, tarPos);

                if (!m_rightTarget.activeInHierarchy)
                {
                    m_rightTarget.SetActive(true);
                }

                m_rightTarget.transform.position = tarPos;
                m_rightTarget.transform.up = hit.normal;
            }
            
            if (Time.time >= beepTime + beepPeriod * (1.0f - ((Time.time - startTime) / 3.0f)) && !m_audioSource.isPlaying)
            {
                beepTime = Time.time;

                m_audioSource.PlayOneShot(m_audioSource.clip);
            }
            
            yield return null;
        } while (startTime + 3.0f > Time.time && !m_shielding);

        if (!m_shielding)
        {
            if (arm == m_leftArm)
            {
                m_leftArmLocked = true;
            }
            else if (arm == m_rightArm)
            {
                m_rightArmLocked = true;
            }

            //Fire
            GameObject ball = m_cannonAmmo.GetObject();
            ball.transform.position = arm.transform.position + arm.transform.forward * 10.0f;

            ball.SetActive(true);

            Rigidbody ballRB = ball.GetComponent<Rigidbody>();
            ballRB.velocity = arm.transform.forward * 200.0f;
        }

        m_firing = false;

        //Wait
        startTime = Time.time;
        do
        {
            if (arm == m_leftArm)
            {
                //Visualize aim

                m_leftLazer.SetPosition(0, arm.transform.position);
                m_leftLazer.SetPosition(1, tarPos);

                if (!m_leftTarget.activeInHierarchy)
                {
                    m_leftTarget.SetActive(true);
                }

                m_leftTarget.transform.position = tarPos;
                m_leftTarget.transform.up = hit.normal;
            }
            else if (arm == m_rightArm)
            {
                //Visualize aim

                m_rightLazer.SetPosition(0, arm.transform.position);
                m_rightLazer.SetPosition(1, tarPos);

                if (!m_rightTarget.activeInHierarchy)
                {
                    m_rightTarget.SetActive(true);
                }

                m_rightTarget.transform.position = tarPos;
                m_rightTarget.transform.up = hit.normal;
            }

            yield return null;
        } while (startTime + 1.0f > Time.time && !m_shielding);

        //Done
        if (arm == m_leftArm)
        {
            if (m_leftTarget.activeInHierarchy)
            {
                m_leftTarget.SetActive(false);
            }

            m_leftLazer.SetPosition(0, Vector3.zero);
            m_leftLazer.SetPosition(1, Vector3.zero);

            m_leftArmFiring = false;
            m_leftArmLocked = false;
        }
        else if (arm == m_rightArm)
        {
            if (m_rightTarget.activeInHierarchy)
            {
                m_rightTarget.SetActive(false);
            }

            m_rightLazer.SetPosition(0, Vector3.zero);
            m_rightLazer.SetPosition(1, Vector3.zero);

            m_rightArmFiring = false;
            m_rightArmLocked = false;
        }

        //Cooldown
        yield return new WaitForSeconds(3.0f);

        yield return null;
    }

    void AimGunArm (GameObject arm)
    {
        if (arm == m_leftArm)
        {
            if (!m_leftArmLocked)
            {
                AimAtTarget(arm, m_player);
            }
            else
            {                
                AimAtTarget(arm, m_leftTarget);
            }
        }
        else if (arm == m_rightArm)
        {
            if (!m_rightArmLocked)
            {
                AimAtTarget(arm, m_player);
            }
            else
            {
                AimAtTarget(arm, m_rightTarget);
            }
        }       
    }

    void AimAtTarget(GameObject arm, GameObject target)
    {
        Vector3 toTarget = target.transform.position - arm.transform.position;

        float a = Vector3.Dot(toTarget.normalized, transform.forward);
        float b = Vector3.Dot(toTarget.normalized, transform.right);
        
        bool aimObstructed = false;
        
        if (arm == m_leftArm)
        {
            if (b > 0.5f)
            {
                aimObstructed = true;
            }
        }
        else if (arm == m_rightArm)
        {
            if (b < -0.5f)
            {
                aimObstructed = true;
            }
        }        

        if (!aimObstructed && a > 0.0f)
        {
            Quaternion tarRot = Quaternion.LookRotation(toTarget.normalized);
            arm.transform.rotation = Quaternion.RotateTowards(arm.transform.rotation, tarRot, m_maxAimSpeed);
            //arm.transform.rotation = Quaternion.Slerp(arm.transform.rotation, tarRot, m_maxAimSpeed * Time.deltaTime);

            if (a > 0.90f && !m_firing)
            {
                FireGunArm(arm);
            }
        }
        else
        {
            Quaternion tarRot = Quaternion.LookRotation(transform.forward);
            arm.transform.rotation = Quaternion.RotateTowards(arm.transform.rotation, tarRot, m_maxAimSpeed);
            //arm.transform.rotation = Quaternion.Slerp(arm.transform.rotation, tarRot, m_maxAimSpeed * Time.deltaTime);
        }
    }

    public void HitWeakPoint (Vector3 normal)
    {
        //ADD CHECK FOR IS PLAYER KICKING!
        if (m_playerController.PlayerIsKicking() && !m_weakPointHit)
        {
            Debug.Log("hit weakpoint!");
            m_weakPointHit = true;

            m_hits++;

            m_playerController.SetAirDashes(m_playerController.GetAirDashes() + 1.5f);
            m_playerController.Bounce(normal, 30.0f);


            if (m_hits < 3)
            {
                BossShield();
            }
            else
            {
                BossShield();
                BossDie();
            }
        }        
    }
}
