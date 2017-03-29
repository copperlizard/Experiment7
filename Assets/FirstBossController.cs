using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player, m_leftArm, m_rightArm, m_leftTarget, m_rightTarget;

    [SerializeField]
    private float m_maxTurnSpeed = 15.0f, m_maxAimSpeed = 15.0f;

    private LineRenderer m_leftLazer, m_rightLazer;

    private Vector3 m_tarPos;

    private bool m_leftArmLocked = false, m_rightArmLocked = false, m_leftArmFiring = false, m_rightArmFiring = false;

    // Use this for initialization
    void Start ()
    {
		if (m_player == null)
        {
            m_player = GameObject.FindGameObjectWithTag("Player");

            if (m_player == null)
            {
                Debug.Log("m_player not found!");
            }
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
    }
	
	// Update is called once per frame
	void Update ()
    {
        FacePlayer();

        //Lock arms when firing...

        AimGunArm(m_leftArm);
        AimGunArm(m_rightArm);        
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
                Debug.Log("left arm fire!");

                m_leftArmFiring = true;
                StartCoroutine(GunArmFire(arm));
            }
        }
        else if (arm == m_rightArm)
        {
            if (!m_rightArmFiring)
            {
                Debug.Log("right arm fire!");

                m_rightArmFiring = true;
                StartCoroutine(GunArmFire(arm));
            }
        }
    }

    IEnumerator GunArmFire (GameObject arm)
    {
        Vector3 tarPos;
        RaycastHit hit;

        float startTime = Time.time;

        do
        {
            if (Physics.Raycast(arm.transform.position, arm.transform.forward, out hit, 1000.0f, ~LayerMask.GetMask("Player", "Boss")))
            {
                tarPos = hit.point;
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

            yield return null;
        } while (startTime + 3.0f > Time.time);       

      
        //Fire
        if (arm == m_leftArm)
        {
            if (m_leftTarget.activeInHierarchy)
            {
                m_leftTarget.SetActive(false);
            }

            m_leftLazer.SetPosition(0, Vector3.zero);
            m_leftLazer.SetPosition(1, Vector3.zero);
        }
        else if (arm == m_rightArm)
        {
            if (m_rightTarget.activeInHierarchy)
            {
                m_rightTarget.SetActive(false);
            }

            m_rightLazer.SetPosition(0, Vector3.zero);
            m_rightLazer.SetPosition(1, Vector3.zero);
        }

        //Wait
        yield return new WaitForSeconds(3.0f);

        //Done
        if (arm == m_leftArm)
        {
            m_leftArmFiring = false;
            m_leftArmLocked = false;
        }
        else if (arm == m_rightArm)
        {
            m_rightArmFiring = false;
            m_rightArmLocked = false;
        }

        yield return null;
    }

    void AimGunArm (GameObject arm)
    {
        Vector3 toPlayer = m_player.transform.position - arm.transform.position;

        float a = Vector3.Dot(toPlayer.normalized, transform.forward);
        if (a > 0.65f)
        {
            Quaternion tarRot = Quaternion.LookRotation(toPlayer.normalized);
            arm.transform.rotation = Quaternion.RotateTowards(arm.transform.rotation, tarRot, m_maxAimSpeed);
            //arm.transform.rotation = Quaternion.Slerp(arm.transform.rotation, tarRot, m_maxAimSpeed * Time.deltaTime);

            if (a > 0.95f)
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
}
