using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float m_maxSpeed = 20.0f, m_turnSpeed = 5.0f, m_groundCheckDist = 0.25f;

    private Animator m_playerAnimator;

    private Camera m_camera;
    private CameraController m_cameraController;

    private Rigidbody m_playerRB;
    
    private RaycastHit m_groundAt;

    private float m_turn = 0.0f, m_jumpCharge = 0.0f, m_speed = 0.0f;

    private bool m_grounded = true;

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
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_playerAnimator.SetFloat("Speed", m_speed / m_maxSpeed);
        m_playerAnimator.SetFloat("Turn", m_turn);
        m_playerAnimator.SetFloat("Falling", m_playerRB.velocity.y);
        m_playerAnimator.SetBool("Grounded", m_grounded);

        m_playerAnimator.speed = 1.0f + Mathf.SmoothStep(0.75f, 1.0f, m_speed);
	}

    public void SetJumpCharge (float charge)
    {
        m_jumpCharge = charge;
    }

    private void GroundCheck ()
    {
        //Debug.DrawRay(transform.position + transform.up * 0.5f, -transform.up, Color.red);
        
        if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out m_groundAt, m_groundCheckDist + 0.5f))
        {
            m_grounded = true;
            AlignWithGround();         
        }     
        else
        {
            m_grounded = false;
        }

        // ADD CHECK FOR STALLED WALL RUN (not grounded!)   
    }

    private void AlignWithGround()
    {
        Debug.DrawRay(m_groundAt.point, m_groundAt.normal, Color.green);

        Vector3 alignedForward = -Vector3.Cross(Vector3.Cross(transform.forward, m_groundAt.normal), m_groundAt.normal);

        Debug.DrawRay(m_groundAt.point, alignedForward, Color.blue);

        Quaternion alignedRot = Quaternion.LookRotation(alignedForward, m_groundAt.normal);
        transform.rotation = Quaternion.Lerp(transform.rotation, alignedRot, 300.0f * Time.deltaTime);

        Vector3 toGround = m_groundAt.point - transform.position;       
        
        if (Vector3.Dot(transform.up, toGround.normalized) > 0.0) // Lift player out of the ground!
        {
            transform.position = m_groundAt.point;
        } 
        
        //Debug.DrawLine(transform.position, transform.position + toGround, Color.blue);
    }

    public void Move (Vector2 move)
    {
        GroundCheck();

        move = move.normalized * Mathf.Min(1.0f, move.magnitude); // Make keyboard input look like joystick input (joystick unaffected)

        m_turn = move.x;

        m_speed = Mathf.Lerp(m_playerRB.velocity.magnitude, m_maxSpeed * move.magnitude, 50.0f * Time.deltaTime);

        Vector3 move3d = new Vector3(move.x, 0.0f, move.y);
        move3d = transform.rotation * move3d;

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
        
        Vector3 vel = transform.forward * m_speed;

        m_playerRB.velocity = vel;
        
        if (move.magnitude < 0.05)
        {
            return;
        }
        else
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move * ((m_speed >= 0.0f)?1.0f:-1.0f), transform.up).normalized, transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * (1.0f - 0.85f * Mathf.SmoothStep(0.0f, 1.0f, m_speed - 0.1f)));
        }
    }

    private void AirMove (Vector2 move)
    {
        if (!m_playerRB.useGravity)
        {
            m_playerRB.useGravity = true;
        }
        
        Vector3 tarForward = -Vector3.Cross(Vector3.Cross(transform.forward, Vector3.up), Vector3.up);

        Debug.DrawRay(transform.position, tarForward, Color.green);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(tarForward, Vector3.up), 0.5f);

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

    public void Jump ()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default")) // push of level
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + collision.contacts[0].normal * 0.1f, 0.1f);
        }
    }
}
