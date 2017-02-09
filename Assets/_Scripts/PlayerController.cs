using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float m_maxSpeed = 20.0f, m_turnSpeed = 5.0f, m_jumpSpeed = 10.0f, m_airDashSpeed = 30.0f, m_dashDuration = 1.0f, m_groundCheckDist = 0.25f, m_minFlightHeight = 1.0f;

    private Animator m_playerAnimator;

    private Camera m_camera;
    private CameraController m_cameraController;

    private Rigidbody m_playerRB;

    private ParticleSystem m_dashParticles;    
    
    private RaycastHit m_groundAt;

    private Vector3 m_groundParallel;
    
    private float m_turn = 0.0f, m_jumpCharge = 0.0f, m_speed = 0.0f, m_flying = 0.0f, m_airDashes = 3.0f;

    private bool m_grounded = true, m_jumping = false, m_airDashing = false, m_airDashCancel = false, m_stalled = false;

	// Use this for initialization
	void Start ()
    {
        m_playerRB = GetComponent<Rigidbody>();
        if (m_playerRB == null)
        {
            Debug.Log("m_playerRB not found!");
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
        m_airDashes = Mathf.Lerp(m_airDashes, 3.0f, 0.1f * Time.deltaTime);

        m_playerAnimator.SetFloat("Speed", m_speed / m_maxSpeed);
        m_playerAnimator.SetFloat("Turn", m_turn);
        m_playerAnimator.SetFloat("Falling", m_playerRB.velocity.y);
        m_playerAnimator.SetFloat("JumpCharge", m_jumpCharge);
        m_playerAnimator.SetFloat("Flying", m_flying);
        m_playerAnimator.SetBool("Grounded", m_grounded);

        m_playerAnimator.speed = 1.0f + Mathf.SmoothStep(0.75f, 1.0f, m_speed);
	}

    public void SetJumpCharge (float charge)
    {
        m_jumpCharge = charge;
    }

    private void GroundCheck ()
    {        
        if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out m_groundAt, m_groundCheckDist + 0.5f))
        {
            if (!m_stalled)
            {
                m_grounded = true;

                // Check for stalled wallrun   
                if (m_playerRB.velocity.magnitude < 0.5f * m_maxSpeed && Vector3.Dot(transform.up, Vector3.up) < 0.5f)
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
        //Debug.DrawRay(m_groundAt.point, m_groundAt.normal, Color.green);

        //Vector3 alignedForward = -Vector3.Cross(Vector3.Cross(transform.forward, m_groundAt.normal), m_groundAt.normal);
        m_groundParallel = -Vector3.Cross(Vector3.Cross(transform.forward, m_groundAt.normal), m_groundAt.normal);

        //Debug.DrawRay(m_groundAt.point, alignedForward, Color.blue);



        //Quaternion alignedRot = Quaternion.LookRotation(alignedForward, m_groundAt.normal);
        Quaternion alignedRot = Quaternion.LookRotation(m_groundParallel, m_groundAt.normal);
        transform.rotation = Quaternion.Lerp(transform.rotation, alignedRot, 3.0f * Time.deltaTime);

        Vector3 toGround = m_groundAt.point - transform.position;       
        
        if (toGround.magnitude > 0.005f) // Lift player out of the ground!
        {
            transform.position = m_groundAt.point;
        }        

        //Debug.DrawLine(transform.position, transform.position + toGround, Color.blue);
    }

    public void Move (Vector2 move)
    {
        if (!m_jumping)
        {
            GroundCheck();
        }       

        move = move.normalized * Mathf.Min(1.0f, move.magnitude); // Make keyboard input look like joystick input (joystick unaffected)

        m_turn = move.x; // For animator

        float tiltFactor = 1.0f;

        Vector3 move3d = new Vector3(move.x, 0.0f, move.y);
        move3d = transform.rotation * move3d;

        tiltFactor *= Mathf.SmoothStep(0.0f, 1.0f, Mathf.InverseLerp(0.0f, 0.8f, Mathf.Abs(Vector3.Dot(move3d, transform.forward))));

        m_speed = Mathf.Lerp(m_playerRB.velocity.magnitude, m_maxSpeed * move.magnitude * tiltFactor, 7.5f * Time.deltaTime);

        //Debug.Log("m_speed == " + m_speed.ToString());

        //Debug.Log(Mathf.SmoothStep(0.0f, 1.0f, Mathf.InverseLerp(0.0f, 0.8f, Mathf.Abs(Vector3.Dot(move3d, transform.forward)))).ToString());

        // Reverse        
        if (Vector3.Dot(move3d, -transform.forward) > 0.45f)
        {
            m_speed = -m_speed;
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

        //Vector3 vel = transform.forward * m_speed;
        Vector3 vel = m_groundParallel.normalized * m_speed;

        m_playerRB.velocity = vel;
        
        if (move.magnitude < 0.05)
        {
            return;
        }
        else
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move * ((m_speed >= 0.0f)?1.0f:-1.0f), transform.up).normalized, transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * (1.0f - 0.75f * Mathf.SmoothStep(0.0f, 1.0f, (m_speed / m_maxSpeed))));
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
            Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move3d, transform.up).normalized, transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * (1.0f - 0.85f * Mathf.SmoothStep(0.0f, 1.0f, m_playerRB.velocity.magnitude / m_maxSpeed)));
        }
    }

    public void AirDash (Vector2 move)
    {
        if (m_airDashes < 1.0f || move.magnitude < 0.1f)
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
                
        float startTime = Time.time, endTime = startTime + m_dashDuration;

        do
        {
            m_playerRB.velocity = move.normalized * Mathf.Lerp(m_playerRB.velocity.magnitude, m_airDashSpeed, 10.0f * Time.deltaTime);
            //Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move, transform.up).normalized, transform.up);
            Quaternion rot = Quaternion.LookRotation(m_playerRB.velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * 2.0f);

            if (Physics.Raycast(transform.position + transform.up * 0.5f, Vector3.Lerp(-transform.up, transform.forward, (m_speed / m_maxSpeed) * 0.5f), out m_groundAt))
            {
                Debug.DrawLine(transform.position, m_groundAt.point, Color.white);

                float fly = Vector3.Dot(transform.forward, m_playerRB.velocity.normalized);
                fly *= Mathf.Min(m_speed / m_maxSpeed, 1.0f);
                fly *= Mathf.Min(Vector3.Distance(transform.position, m_groundAt.point) / m_minFlightHeight, 1.0f);
                fly = Mathf.Clamp(fly, 0.0f, 1.0f);
                m_flying = Mathf.Lerp(m_flying, fly, 20.0f * Time.deltaTime);
            }
            else
            {
                float fly = Vector3.Dot(transform.forward, m_playerRB.velocity.normalized);
                fly *= Mathf.Min(m_speed / m_maxSpeed, 1.0f);
                fly = Mathf.Clamp(fly, 0.0f, 1.0f);
                m_flying = Mathf.Lerp(m_flying, fly, 20.0f * Time.deltaTime);
            }

            yield return null;
        } while (Time.time <= endTime && !m_airDashCancel);

        do
        {
            if (Physics.Raycast(transform.position + transform.up * 0.5f, Vector3.Lerp(-transform.up, transform.forward, (m_speed / m_maxSpeed) * 0.5f), out m_groundAt))
            {
                Debug.DrawLine(transform.position, m_groundAt.point, Color.white);

                m_flying *= Mathf.Max(Vector3.Dot(transform.forward, m_playerRB.velocity.normalized), 0.0f);
                m_flying *= Mathf.Min(m_speed / m_maxSpeed, 1.0f);
                m_flying *= Mathf.Min(Vector3.Distance(transform.position, m_groundAt.point) / m_minFlightHeight, 1.0f);
                m_flying = Mathf.Lerp(m_flying, 0.0f, 10.0f * Time.deltaTime);
            }
            else
            {
                m_flying *= Mathf.Max(Vector3.Dot(transform.forward, m_playerRB.velocity.normalized), 0.0f);
                m_flying *= Mathf.Min(m_speed / m_maxSpeed, 1.0f);                
                m_flying = Mathf.Lerp(m_flying, 0.0f, 10.0f * Time.deltaTime);
            }

            if (m_flying <= 0.05f)
            {
                m_flying = 0.0f;
            }

            yield return null;
        } while (m_flying > 0.0f && !m_airDashCancel);
        
        m_airDashing = false;
        m_airDashCancel = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default")) // push of level
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + collision.contacts[0].normal * 0.1f, 0.1f);
        }
    }

    public bool PlayerIsGrounded()
    {
        return m_grounded;
    }
}
