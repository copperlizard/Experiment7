using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float m_maxSpeed = 20.0f, m_turnSpeed = 5.0f, m_jumpSpeed = 10.0f, m_airDashSpeed = 30.0f, 
        m_dashDuration = 1.0f, m_groundCheckDist = 0.25f, m_minFlightHeight = 1.0f, m_flightTransitionRate = 20.0f,
        m_runCycleLegOffset = 0.0f;

    [SerializeField]
    private GameObject m_playerShield;

    [SerializeField]
    private AudioClip m_airDashSound;

    private Animator m_playerAnimator;

    private Camera m_camera;
    private CameraController m_cameraController;

    private AudioSource m_footStepsSoundEffectSource;

    private Rigidbody m_playerRB;

    private ParticleSystem m_dashParticles;    
    
    private RaycastHit m_groundAt;

    private Vector3 m_groundParallel;

    private float m_turn = 0.0f, m_jumpCharge = 0.0f, m_speed = 0.0f, m_flying = 0.0f, m_airDashes = 3.0f, m_shieldEnergy = 1.0f, m_speedMod = 1.0f;

    private bool m_grounded = true, m_jumping = false, m_airDashing = false, m_airDashCancel = false, m_stalled = false, m_shielding = false, m_freeFly = false;

	// Use this for initialization
	void Start ()
    {        
        if (m_playerShield == null)
        {
            Debug.Log("m_playerShield not assigned!");
        }

        if (m_airDashSound == null)
        {
            Debug.Log("m_airDashSound not assigned!");
        }

        m_playerRB = GetComponent<Rigidbody>();
        if (m_playerRB == null)
        {
            Debug.Log("m_playerRB not found!");
        }

        m_footStepsSoundEffectSource = GetComponent<AudioSource>();
        if (m_footStepsSoundEffectSource == null)
        {
            Debug.Log("m_playerAudioSource not found!");
        }

        m_playerAnimator = GetComponentInChildren<Animator>();
        if (m_playerRB == null)
        {
            Debug.Log("m_playerRB not found!");
        }

        m_camera = Camera.main;
        if (m_camera == null)
        {
            Debug.Log("m_camera not found!");
        }

        m_cameraController = m_camera.GetComponent<CameraController>();
        if (m_cameraController == null)
        {
            Debug.Log("m_cameraController not found!");
        }

        m_dashParticles = GetComponentInChildren<ParticleSystem>();        
    }
	
	// Update is called once per frame
	void Update ()
    {        
        m_airDashes = Mathf.Min(3.0f, m_airDashes + 0.1f * Time.deltaTime);

        if (!m_shielding)
        {
            m_shieldEnergy = Mathf.Min(1.0f, m_shieldEnergy + 0.15f * Time.deltaTime);
        }        

        m_playerAnimator.SetFloat("Speed", m_speed / m_maxSpeed);
        m_playerAnimator.SetFloat("Turn", m_turn);
        //m_playerAnimator.SetFloat("Falling", m_playerRB.velocity.y);
        m_playerAnimator.SetFloat("Falling", Mathf.Lerp(0.0f, transform.InverseTransformVector(m_playerRB.velocity).y, 1.0f - m_flying));
        m_playerAnimator.SetFloat("JumpCharge", m_jumpCharge * (1.0f - Mathf.InverseLerp(0.0f, 0.2f, Mathf.Abs(m_speed) / m_maxSpeed)));
        m_playerAnimator.SetFloat("Flying", m_flying);
        m_playerAnimator.SetBool("Grounded", m_grounded);

        m_playerAnimator.speed = 1.0f + Mathf.SmoothStep(0.75f, 1.0f, m_speed);
        
        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                m_playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_runCycleLegOffset, 1);
        float jumpLeg = (runCycle < 0.5f ? 1 : -1) * Mathf.Clamp((m_playerRB.velocity.magnitude / m_maxSpeed), 0.0f, 1.0f);
        if (m_grounded)
        {
            m_playerAnimator.SetFloat("JumpLeg", jumpLeg);
        }

        //Foot sounds
        if ((runCycle <= 0.1f || (runCycle >= 0.45f && runCycle <= 0.55f)) && !m_footStepsSoundEffectSource.isPlaying && m_grounded)
        {
            m_footStepsSoundEffectSource.pitch = Random.Range(0.9f, 1.1f);
            //m_footStepsSoundEffectSource.PlayOneShot(m_footStepsSoundEffectSource.clip, Mathf.Lerp(0.0f, 1.0f, m_playerRB.velocity.magnitude));
            m_footStepsSoundEffectSource.PlayOneShot(m_footStepsSoundEffectSource.clip, Mathf.Abs(m_speed / m_maxSpeed));
            //m_footStepsSoundEffectSource.PlayOneShot(m_footStepsSoundEffectSource.clip, 1.0f);
        }

        m_playerAnimator.SetBool("Shielding", m_shielding);
        if (m_shielding)
        {
            m_playerAnimator.SetLayerWeight(1, Mathf.Lerp(m_playerAnimator.GetLayerWeight(1), 1.0f, 10.0f * Time.deltaTime));
        }
        else if (m_playerAnimator.GetLayerWeight(1) > 0.0f)
        {
            if (m_playerAnimator.GetLayerWeight(1) < 0.001f)
            {
                m_playerAnimator.SetLayerWeight(1, 0.0f);
            }
            else
            {
                m_playerAnimator.SetLayerWeight(1, Mathf.Lerp(m_playerAnimator.GetLayerWeight(1), 0.0f, 10.0f * Time.deltaTime));
            }
        }
    }

    public void SetJumpCharge (float charge)
    {
        m_jumpCharge = charge;
    }

    private IEnumerator FallRecover ()
    {
        //Debug.Log("fallen!");

        float startmod = m_speedMod;
        float stagger = 0.5f;
        m_speedMod -= stagger;

        if (m_speedMod < 0.0f)
        {
            m_speedMod = 0.0f;
        }

        while (stagger > 0.0f)
        {
            float delta = 0.25f * Time.deltaTime;
            m_speedMod += delta;
            stagger -= delta;
            yield return null;
        }

        m_speedMod = startmod;

        //Debug.Log("fall recovered!");

        yield return null;
    }

    private void GroundCheck ()
    {
        if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out m_groundAt, m_groundCheckDist + 0.5f, LayerMask.NameToLayer("PlayerBody"), QueryTriggerInteraction.Ignore))
        {
            
            if (!m_grounded && m_playerRB.velocity.y <= -m_maxSpeed * 0.5f) // Already falling fast
            {
                //Debug.Log("fast fall!");

                if (Vector3.Dot(m_groundAt.normal, Vector3.up) > 0.5f)
                {
                    m_grounded = true;
                    m_stalled = false;
                    StartCoroutine(FallRecover());
                }
            }
            else if (!m_stalled)
            {
                m_grounded = true;

                // Check for stalled wallrun   
                if (m_speed < 0.5f * m_maxSpeed && Vector3.Dot(transform.up, Vector3.up) < 0.5f)
                {
                    m_grounded = false;
                    m_stalled = true;
                }                
            }
            else
            {
                if (Vector3.Dot(m_groundAt.normal, Vector3.up) > 0.5f)
                {
                    m_grounded = true;
                    m_stalled = false;
                }
            }            

            AlignWithGround();
        }     
        else
        {
            m_grounded = false;
        }

        return;
    }

    private void AlignWithGround()
    {
        if (m_airDashing)
        {
            return;
        }

        //Debug.DrawRay(m_groundAt.point, m_groundAt.normal, Color.green);

        m_groundParallel = -Vector3.Cross(Vector3.Cross(transform.forward, m_groundAt.normal), m_groundAt.normal);

        //Debug.DrawRay(m_groundAt.point, alignedForward, Color.blue);
        
        Quaternion alignedRot = Quaternion.LookRotation(m_groundParallel, m_groundAt.normal);
        transform.rotation = Quaternion.Lerp(transform.rotation, alignedRot, 10.0f * Time.deltaTime);

        Vector3 toGround = m_groundAt.point - transform.position;       
        
        if (toGround.magnitude > 0.005f) // Lift player out of the ground!
        {
            transform.position = m_groundAt.point;
        }        

        //Debug.DrawLine(transform.position, transform.position + toGround, Color.blue);
    }

    public void Move (Vector2 move)
    {
        if (!m_jumping && !m_freeFly)
        {
            GroundCheck();
        }

        move = move.normalized * Mathf.Min(1.0f, move.magnitude); // Make keyboard input look like joystick input (joystick unaffected)

        m_turn = move.x; // For animator        

        //Player relative movement
        Vector3 move3d = new Vector3(move.x, 0.0f, move.y);
        move3d = transform.rotation * move3d;

        //Add camera relativity (a bit smoother, but causes issues with camera intersections steering the player...)       
        //Vector3 camDir = transform.InverseTransformDirection(Vector3.ProjectOnPlane(m_camera.transform.forward, transform.up).normalized);        
        //Quaternion camRot = Quaternion.LookRotation(camDir);       
        //move3d = camRot * move3d;

        //Slow player in tight turns
        float tiltFactor = Mathf.SmoothStep(0.0f, 1.0f, Mathf.InverseLerp(0.0f, 0.65f, Mathf.Abs(Vector3.Dot(move3d, transform.forward))));

        //m_speed = Mathf.Lerp(m_playerRB.velocity.magnitude, m_maxSpeed * move.magnitude * tiltFactor * m_speedMod, 3.5f * Time.deltaTime);
        //m_speed = Mathf.Lerp(m_speed, m_maxSpeed * move.magnitude * tiltFactor * m_speedMod, 3.5f * Time.deltaTime);
        //m_speed = Mathf.Lerp(m_speed, m_maxSpeed * move.magnitude * tiltFactor, 7.5f * Time.deltaTime);

        // Reverse        
        if (Vector3.Dot(move3d.normalized, -transform.forward) > 0.45f)
        {
            //m_speed = -Mathf.Lerp(m_playerRB.velocity.magnitude, (m_maxSpeed * 0.5f) * move.magnitude * tiltFactor * m_speedMod, 3.5f * Time.deltaTime);
            m_speed = Mathf.Lerp(m_speed, -1.0f * (m_maxSpeed * 0.5f) * move.magnitude * tiltFactor * m_speedMod, 3.5f * Time.deltaTime);
            //m_speed = m_speed * 0.5f;
            //m_speed = -m_speed;
        }
        else
        {
            m_speed = Mathf.Lerp(m_speed, m_maxSpeed * move.magnitude * tiltFactor * m_speedMod, 3.5f * Time.deltaTime);
        }
        
        if (m_shielding)
        {
            ShieldMove(move);
            return;
        }

        if (m_freeFly)
        {
            FlyMove(move);
            return;
        }

        if (m_grounded)
        {
            GroundMove(move3d);
        }
        else
        {
            AirMove(move);
        }        
    }

    private void GroundMove (Vector3 move)
    {
        if (m_playerRB.useGravity)
        {
            m_playerRB.useGravity = false;
        }

        Vector3 vel = m_groundParallel.normalized * m_speed;
        
        m_playerRB.velocity = vel;
        
        if (move.magnitude < 0.05)
        {
            return;
        }
        else
        {
            //Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move * ((m_speed >= 0.0f)?1.0f:-1.0f), transform.up).normalized, transform.up);
            Quaternion rot = Quaternion.LookRotation(move * ((m_speed >= 0.0f) ? 1.0f : -1.0f), transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * m_speedMod * (1.0f - 0.75f * Mathf.SmoothStep(0.0f, 1.0f, (m_speed / m_maxSpeed))));
        }
    }

    private void AirMove (Vector2 move)
    {
        if (!m_playerRB.useGravity)
        {
            m_playerRB.useGravity = true;
        }
        
        Vector3 tarForward = -Vector3.Cross(Vector3.Cross(transform.forward, Vector3.up), Vector3.up);

        //Debug.DrawRay(transform.position, tarForward, Color.green);

        if (m_airDashing)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(m_playerRB.velocity.normalized), 0.5f);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(tarForward, Vector3.up), 0.5f);
        }

        Vector3 move3d = new Vector3(move.x, 0.0f, move.y);
        move3d = transform.rotation * move3d;

        if (move3d.magnitude < 0.05)
        {
            return;
        }
        else
        {
            //Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move3d, transform.up).normalized, transform.up);
            //Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move * ((m_speed >= 0.0f) ? 1.0f : -1.0f), transform.up).normalized, transform.up);
            Quaternion rot = Quaternion.LookRotation(move3d, transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * m_speedMod * 0.5f * (1.0f - 0.775f * Mathf.SmoothStep(0.0f, 1.0f, (m_speed / m_maxSpeed))));
        }
    }

    private void FlyMove(Vector2 move)
    {
        if (m_playerRB.useGravity)
        {
            m_playerRB.useGravity = false;
        }

        //Vector3 move3d = new Vector3(move.y, 0.0f, move.x); //local

        Vector3 heading = transform.TransformDirection(Quaternion.Euler(move.y, 0.0f, 0.0f) * Vector3.forward);
        Vector3 headingUp = transform.TransformDirection(Quaternion.Euler(0.0f, 0.0f, -move.x * 2.0f) * Vector3.up);
        
        
        //Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move * ((m_speed >= 0.0f) ? 1.0f : -1.0f), transform.up).normalized, transform.up);
        Quaternion rot = Quaternion.LookRotation(heading, headingUp);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * m_speedMod * (1.0f - 0.75f * Mathf.SmoothStep(0.0f, 1.0f, (m_speed / m_maxSpeed))));
        

        m_playerRB.velocity = Vector3.Lerp(m_playerRB.velocity, transform.forward * m_maxSpeed * 3.0f, 3.0f * Time.deltaTime);
    }

    public void AirDash (Vector2 move)
    {
        if (m_airDashes < 1.0f || move.magnitude < 0.1f || m_freeFly)
        {
            return;
        }

        if (m_airDashing )
        {
            m_airDashCancel = true;
            return;
        }

        m_airDashes -= 1.0f;
                
        m_dashParticles.Play();

        move = move.normalized * Mathf.Min(1.0f, move.magnitude); // Make keyboard input look like joystick input (joystick unaffected)

        Vector3 move3d = new Vector3(move.x, 0.0f, move.y);
        move3d = transform.rotation * move3d;

        StartCoroutine(AirDashing(move3d));
    }

    private IEnumerator AirDashing (Vector3 move)
    {
        m_airDashing = true;

        m_footStepsSoundEffectSource.PlayOneShot(m_airDashSound);

        float startTime = Time.time, endTime = startTime + m_dashDuration;

        do
        {
            m_playerRB.velocity = move.normalized * Mathf.Lerp(m_playerRB.velocity.magnitude, m_airDashSpeed, 10.0f * Time.deltaTime);            
            Quaternion rot = Quaternion.LookRotation(m_playerRB.velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * 2.0f);

            //float fly = Mathf.Lerp(m_flying, 1.0f, m_flightTransitionRate * Time.deltaTime);
            float fly = 1.0f;

            if (Physics.Raycast(transform.position + transform.up * 0.5f, Vector3.Lerp(-transform.up, transform.forward, (m_speed / m_maxSpeed) * 0.5f), out m_groundAt))
            {
                Debug.DrawLine(transform.position, m_groundAt.point, Color.white);
                
                fly *= Mathf.Min(Vector3.Distance(transform.position, m_groundAt.point) / m_minFlightHeight, 1.0f);
            }            

            fly = Mathf.Clamp(fly, 0.0f, 1.0f);
            m_flying = Mathf.Lerp(m_flying, fly, m_flightTransitionRate * Time.deltaTime);

            yield return null;
        } while (Time.time <= endTime && !m_airDashCancel && !m_freeFly);

        do
        {
            if (Physics.Raycast(transform.position + transform.up * 0.5f, Vector3.Lerp(-transform.up, transform.forward, (m_speed / m_maxSpeed) * 0.5f), out m_groundAt))
            {
                Debug.DrawLine(transform.position, m_groundAt.point, Color.white);

                m_flying *= Mathf.Min(Vector3.Distance(transform.position, m_groundAt.point) / m_minFlightHeight, 1.0f);
            }

            m_flying = Mathf.Lerp(m_flying, 0.0f, m_flightTransitionRate * Time.deltaTime);            

            yield return null;
        } while (m_flying > 0.05f && !m_airDashCancel && !m_freeFly);

        if (!m_freeFly)
        {
            m_flying = 0.0f;
        }
        
        m_airDashing = false;
        m_airDashCancel = false;
        yield return null;
    }

    public void FreeFly ()
    {
        Debug.Log("hello!");

        if (!m_freeFly)
        {
            m_freeFly = true;

            ParticleSystem.MainModule main = m_dashParticles.main;
            main.loop = true;
            m_dashParticles.Play();
            StartCoroutine(FreeFlying());
        }
        else
        {
            m_freeFly = false;
        }        
    }

    private IEnumerator FreeFlying ()
    {
        while (m_freeFly)
        {
            Debug.Log("flying!");
            if (m_flying < 1.0f)
            {
                m_flying = Mathf.Lerp(m_flying, 1.0f, m_flightTransitionRate * Time.deltaTime);

                if (m_flying > 0.99f)
                {
                    m_flying = 1.0f;
                }
            }
            yield return null;
        }

        while (m_flying > 0.0f)
        {
            Debug.Log("END flying!");
            m_flying = Mathf.Lerp(m_flying, 0.0f, m_flightTransitionRate * Time.deltaTime);

            if (m_flying < 0.01f)
            {
                m_flying = 0.0f;
            }
            
            yield return null;
        }

        yield return null;
    }

    public void Jump ()
    {
        if (!m_grounded)
        {
            return;
        }

        m_grounded = false;
        
        StartCoroutine(Jumping());

        float momentum = m_speed / m_maxSpeed;

        Vector3 jumpV = Vector3.Lerp(transform.up, transform.up, momentum * 0.5f * Time.deltaTime);
        m_playerRB.velocity += jumpV * m_jumpSpeed * m_jumpCharge;
    }

    private IEnumerator Jumping ()
    {
        m_jumping = true;
        yield return new WaitForSeconds(0.1f);
        m_jumping = false;
        yield return null;
    }

    public void Shield (bool state)
    {
        if (m_shieldEnergy <= 0.0f)
        {
            m_shielding = false;
            return;
        }

        if (state && !m_shielding)
        {
            StartCoroutine(Shielding());
            m_shielding = true;
            m_grounded = false;
        }
        else if (!state)
        {
            m_shielding = false;
        }
    }

    private void ShieldMove (Vector2 move)
    {
        if (m_playerRB.useGravity)
        {
            m_playerRB.useGravity = false;
        }

        move = move.normalized * Mathf.Min(1.0f, move.magnitude); // Make keyboard input look like joystick input (joystick unaffected)

        m_turn = move.x; // For animator        

        //Playre relative movement
        Vector3 move3d = new Vector3(move.x, 0.0f, move.y);
        move3d = transform.rotation * move3d;

        m_playerRB.velocity = Vector3.Lerp(m_playerRB.velocity, move3d * 3.0f, 3.0f * Time.deltaTime);

        m_speed = m_playerRB.velocity.magnitude;

        if (move3d.magnitude < 0.05)
        {
            return;
        }
        else
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move3d, transform.up).normalized, transform.up);
            //Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move * ((m_speed >= 0.0f) ? 1.0f : -1.0f), transform.up).normalized, transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * 0.5f * m_speedMod * (1.0f - 0.775f * Mathf.SmoothStep(0.0f, 1.0f, (m_speed / m_maxSpeed))));
        }
    }

    private IEnumerator Shielding()
    {
        if (m_playerRB.useGravity)
        {
            m_playerRB.useGravity = false; //letting ground check reset gravity...
        }

        //Visualize shield
        m_playerShield.SetActive(true);

        do //wait for enery to run out or button release
        {
            if (m_playerShield.transform.localScale.x < 1.0f)
            {
                m_playerShield.transform.localScale = Vector3.Lerp(m_playerShield.transform.localScale, Vector3.one, 7.5f * Time.deltaTime);
            }

            //m_shieldEnergy = Mathf.Lerp(m_shieldEnergy, 0.0f, 0.5f * Time.deltaTime);
            m_shieldEnergy -= 0.3f * Time.deltaTime;

            //Debug.Log("m_shieldEnergy == " + m_shieldEnergy.ToString());

            yield return null;
        } while (m_shielding && m_shieldEnergy > 0.01f);

        m_shielding = false;
        

        //DeVisualize shield
        m_playerShield.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        m_playerShield.SetActive(false);

        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default")) // push of level
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + collision.contacts[0].normal * 0.1f, 0.1f);
        }
    }

    public bool PlayerIsGrounded ()
    {
        return m_grounded;
    }

    public bool PlayerIsShielded ()
    {
        return m_shielding;
    }

    public float GetShieldEnergy ()
    {
        return m_shieldEnergy;
    }

    public float GetSpeed ()
    {
        return m_playerRB.velocity.magnitude;
        //return m_speed;
    }

    public float GetMaxSpeed()
    {
        return m_maxSpeed;
    }

    public float GetAirDashes ()
    {
        return m_airDashes;
    }
    
    public void AdjustSpeedMod (float delta)
    {
        m_speedMod += delta;
        m_speedMod = Mathf.Clamp(m_speedMod, 0.0f, 1.0f);
    } 
}
