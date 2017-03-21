using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTurret : MonoBehaviour
{
    [SerializeField]
    private GameObject m_spinFX;

    [SerializeField]
    private float m_turretRotationSpeed = 15.0f, m_turretRotationAccellerationRate = 5.0f, m_fireRate = 1.0f, m_fireSpeed = 30.0f,
        m_turretSpread = 1.5f, m_iceThawRate = 1.0f, m_iceSprayRadius = 0.25f;

    private GameObject m_player;
    private PlayerController m_playerController;
    private Rigidbody m_playerRB;

    private BasicRotator m_turretRotator;

    private AudioSource m_audioSource;

    private ObjectPool m_bulletPool, m_playerBodyProjectorPool, m_defaultProjectorPool;

    private List<ProjectOnLayer> m_activeProjectors = new List<ProjectOnLayer>();
    
    private float m_stackedSlowEffect = 0.0f, m_lastFiredAt = 0.0f;

    private bool m_playerDetected = false, m_lockOn = false, m_firing = false;

	// Use this for initialization
	void Start ()
    {
        if (m_spinFX == null)
        {
            Debug.Log("m_spinFX not assigned!");
        }

        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            Debug.Log("m_audioSource not found!");
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

        m_playerRB = m_player.GetComponent<Rigidbody>();
        if (m_playerRB == null)
        {
            Debug.Log("m_playerRB not found!");
        }

        m_turretRotator = GetComponentInChildren<BasicRotator>();
        if (m_turretRotator == null)
        {
            Debug.Log("m_turretRotator not found!");
        }

        m_bulletPool = GetComponents<ObjectPool>()[0];
        if (m_bulletPool == null)
        {
            Debug.Log("m_bulletPool not found!");
        }

        m_playerBodyProjectorPool = GetComponents<ObjectPool>()[1];
        if (m_bulletPool == null)
        {
            Debug.Log("m_playerBodyProjectorPool not found!");
        }

        m_defaultProjectorPool = GetComponents<ObjectPool>()[2];
        if (m_bulletPool == null)
        {
            Debug.Log("m_defaultProjectorPool not found!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_stackedSlowEffect > 0.0f)
        {
            float thaw = Mathf.Max(m_stackedSlowEffect - m_iceThawRate * Time.deltaTime, 0.0f);
            float delta = m_stackedSlowEffect - thaw;
            m_stackedSlowEffect -= delta;
            m_playerController.AdjustSpeedMod(delta);
        }        
                
        for(int i = 0; i < m_activeProjectors.Count; i++)
        {
            m_activeProjectors[i].SetProjectorSize(Mathf.Lerp(m_activeProjectors[i].GetProjectorSize(), 0.0f, m_iceThawRate * Time.deltaTime));

            m_activeProjectors[i].SetProjectorColor(Color.white * (m_activeProjectors[i].GetProjectorSize() / 0.25f));

            if (m_activeProjectors[i].GetProjectorSize() <= 0.01f)
            {
                m_activeProjectors[i].SetProjectorSize(0.25f);
                m_activeProjectors[i].gameObject.SetActive(false);
                m_activeProjectors.RemoveAt(i);
                i--;
            }                
        }
        
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
            if (m_spinFX.activeInHierarchy)
            {
                m_spinFX.SetActive(false);
            }

            m_turretRotator.SetRotation(new Vector3(0.0f, 0.0f, Mathf.Lerp(m_turretRotator.GetRotation().z, 0.0f, m_turretRotationAccellerationRate * Time.deltaTime)));
        }
	}

    private Vector3 PredictAim () //returns line to player not player pos
    {       
        if (m_playerRB.velocity.magnitude < 5.0f) //too slow, no prediction
        {            
            return ((m_player.transform.position + m_player.transform.up * 1.3f) - transform.position);
        }
        

        Vector3 A = (m_player.transform.position + m_player.transform.up * 1.3f) + m_playerRB.velocity.normalized; 
        Vector3 p = (m_player.transform.position + m_player.transform.up * 1.3f);

        //Debug.DrawLine(A, p, Color.white);

        float dif = 100.0f;
        int loops = 0, looplim = 30;
        while (Mathf.Abs(dif) > 0.5f && loops <= looplim)
        {
            Vector3 pA = A - p;

            //Debug.DrawLine(p, p + pA, Color.yellow * Mathf.SmoothStep(0.5f, 1.0f, (loops / looplim)));

            Vector3 tA = A - transform.position;

            float t = pA.magnitude / m_playerRB.velocity.magnitude; //time for player to reach A            
            //Debug.Log("t == " + t.ToString() + " ; pA.magnitude == " + pA.magnitude.ToString() + " ; m_playerRB.velocity.magnitude == " + m_playerRB.velocity.magnitude.ToString());

            Vector3 bulletMove = ((tA.normalized * m_fireSpeed) * t); //bullet path in t

            Debug.DrawLine(transform.position, transform.position + bulletMove, Color.red * Mathf.SmoothStep(0.5f, 1.0f, (loops / looplim)));

            dif = ((transform.position + bulletMove) - A).magnitude;
            
            
            if (Vector3.Dot((A - transform.position).normalized, (A - (transform.position + bulletMove)).normalized) < -0.9f)
            {
                dif = -dif;
            } 
            
            
            A += m_playerRB.velocity.normalized * Mathf.Min(dif, 3.0f); //push A

            loops++;
        }

        //Debug.Log("dif == " + dif.ToString());
        //Debug.Log("loops == " + loops.ToString());

        Debug.DrawLine(transform.position, A, Color.green);
        Debug.DrawLine(p, A, Color.black);

        Vector3 toPredPlayerPos = (A - transform.position).normalized;
        float jerk = Mathf.SmoothStep(0.0f, 1.0f, (Vector3.Dot(transform.forward, toPredPlayerPos) + 1.0f) * 0.5f);
        Vector3 aim = Vector3.Lerp(transform.forward, toPredPlayerPos, jerk);

        //Debug.Log("jerk == " + jerk.ToString());

        return aim;

        /*
        Vector3 A = (m_player.transform.position + m_player.transform.up * 1.3f); //offset to aim at chest
        Vector3 P = transform.position; //account for fire pos offset later
        Vector3 AB = (m_player.transform.position + m_playerRB.velocity.normalized * 500.0f) - A; //B is A+500m in current velocity dir
        Vector3 AP = (P - A); 

        Vector3 cp = A + Vector3.Dot(AP, AB) / Vector3.Dot(AB, AB) * AB; //project fire pos onto player "velocity" vector

        Debug.DrawLine(A, A + AB.normalized * 500.0f, Color.blue);
        Debug.DrawLine(P, cp, Color.yellow);

        float dif = 100.0f;
        int loops = 0, looplim = 100;
        while (Mathf.Abs(dif) > 1.0f && loops <= looplim)
        {
            Vector3 Pcp = cp - P; //fire pos to cp
            Vector3 Acp = cp - A; //player to cp

            float t = Acp.magnitude / m_playerRB.velocity.magnitude; //time for player to reach cp
            Debug.Log("Acp.magnitude == " + Acp.magnitude.ToString());
            //Debug.Log("m_playerRB.velocity.magnitude == " + m_playerRB.velocity.magnitude.ToString());
            //Debug.Log("t == " + t.ToString());

            Vector3 bulletMove = ((Pcp.normalized * m_fireSpeed) * t); //bullet path in t

            Debug.DrawLine(P, P + bulletMove, Color.red);

            dif = Pcp.magnitude - bulletMove.magnitude; //bullet dist from cp, accounting for fire pos offset
            
            if (Vector3.Dot(((cp + AB.normalized * dif) - A).normalized, m_playerRB.velocity.normalized) < -0.9f)
            {
                Debug.Log("cp would be behind player!");
                dif = Mathf.Abs(dif);
            }

            cp += AB.normalized * dif; //push cp along AB by dif

            loops++;
        }

        
        Debug.Log("dif == " + dif.ToString());
        Debug.Log("loops == " + loops.ToString());
        Debug.DrawLine(A, cp, Color.black);
        Debug.DrawLine(P, cp, Color.green);

        Debug.Log("(cp - P).magnitude == " + (cp - P).magnitude.ToString());

        return (cp - P).normalized;
        */
    }

    private void FacePlayer ()
    {
        Vector3 toPlayer = ((m_player.transform.position + m_player.transform.up * 1.3f) - transform.position);

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
        Vector3 toPlayer = PredictAim();

        if (!m_spinFX.activeInHierarchy)
        {
            m_spinFX.SetActive(true);
        }
        
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
            m_turretRotator.SetRotation(new Vector3(0.0f, 0.0f, Mathf.Lerp(z, m_turretRotationSpeed, m_turretRotationAccellerationRate * Time.deltaTime)));
        }
    }

    private void Fire ()
    {
        Vector3 toPlayer = PredictAim();
        
        transform.rotation = Quaternion.LookRotation(toPlayer.normalized);

        if (Time.time > m_lastFiredAt + m_fireRate)
        {
            //Debug.Log("Firing bullet!");

            FreezeTurretBullet bullet = m_bulletPool.GetObject().GetComponent<FreezeTurretBullet>();
            
            if (bullet != null)
            {
                bullet.transform.position = transform.position + transform.forward;
                bullet.gameObject.SetActive(true);
                bullet.SetTurret(this);
                
                bullet.Fire(transform.rotation * Quaternion.Euler(Random.Range(-m_turretSpread, m_turretSpread), Random.Range(-m_turretSpread, m_turretSpread), Random.Range(-m_turretSpread, m_turretSpread)), m_fireSpeed);
                m_audioSource.PlayOneShot(m_audioSource.clip);
                m_lastFiredAt = Time.time;
            }
            else
            {
                //Debug.Log("error getting freeze turret bullet!");
            }
        }
    }

    /*
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(debugCenter, debugRadius);
    }
    */

    //private Vector3 debugCenter;
    //private float debugRadius;
    public void SprayIce (Transform bullet)
    {
        //Debug.Log("spray ice at " + bullet.position.ToString() + "!");

        Collider[] hitColliders = Physics.OverlapSphere(bullet.position, m_iceSprayRadius, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        //debugCenter = bullet.position;
        //debugRadius = m_iceSprayRadius;
        //Time.timeScale = 0.0f;

        bool defSpray = false, playerSpray = false;

        Transform playerHitTransform = null;
        for (int i = 0; i < hitColliders.Length; i++)  // CHECK FOR STUCK SLOWBALLS AND MOVE TO DEFAULT LAYER!!!
        {
            //Debug.Log("ice overlap with " + hitColliders[i].name);
            Debug.DrawLine(bullet.position, hitColliders[i].transform.position, Color.red);
            
            if (hitColliders[i].tag == "PlayerBody")
            {
                //Debug.Log("ice hit player!");
                playerHitTransform = hitColliders[i].transform;
                playerSpray = true;
            }
            else if (hitColliders[i].gameObject.layer == LayerMask.GetMask("Defualt"))
            {
                //Debug.Log("ice hit level!");

                defSpray = true;
            }
        }

        if (playerSpray)
        {
            ProjectOnLayer sprayProjector = m_playerBodyProjectorPool.GetObject().GetComponent<ProjectOnLayer>();
            sprayProjector.transform.position = bullet.position;
            sprayProjector.transform.rotation = bullet.rotation;
            sprayProjector.transform.parent = playerHitTransform;
            sprayProjector.SetProjectorColor(Color.white);            
            sprayProjector.gameObject.SetActive(true);

            m_activeProjectors.Add(sprayProjector);

            if (m_playerController.GetSpeedMod() >= 0.1f)
            {
                m_stackedSlowEffect += 0.1f;
                m_playerController.AdjustSpeedMod(-0.1f);
            }

            //Debug.Log("PlayerBody Projector added!");
        }

        if (defSpray)
        {
            ProjectOnLayer sprayProjector = m_defaultProjectorPool.GetObject().GetComponent<ProjectOnLayer>();
            sprayProjector.transform.position = bullet.position;
            sprayProjector.transform.rotation = bullet.rotation;
            sprayProjector.SetProjectorColor(Color.white);
            sprayProjector.gameObject.SetActive(true);

            m_activeProjectors.Add(sprayProjector);

            //Debug.Log("default Projector added!");
        }
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
