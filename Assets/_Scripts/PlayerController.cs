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

    private float m_turn = 0.0f;

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
        float speed = m_playerRB.velocity.magnitude / m_maxSpeed;
        m_playerAnimator.SetFloat("Speed", speed);
        m_playerAnimator.SetFloat("Turn", m_turn);

        m_playerAnimator.speed = 1.0f + Mathf.SmoothStep(0.75f, 1.0f, speed);
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
        transform.rotation = Quaternion.Lerp(transform.rotation, alignedRot, 0.3f);

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
        
        if (m_grounded)
        {
            GroundMove(move);
        }
        else
        {
            AirMove(move);
        }        
    }

    private void GroundMove (Vector2 move)
    {
        if (m_playerRB.useGravity)
        {
            m_playerRB.useGravity = false;
        }

        m_turn = move.x;

        Vector3 move3d = new Vector3(move.x, 0.0f, move.y);
        //move3d = Quaternion.Euler(0.0f, m_camera.transform.rotation.eulerAngles.y, 0.0f) * move3d;
        move3d = transform.rotation * move3d;
        
        float speed = Mathf.SmoothStep(m_playerRB.velocity.magnitude, m_maxSpeed, 0.3f);
        
        // Reverse        
        if (Vector3.Dot(move3d, -transform.forward) >= 0.45f)
        {
            speed = -speed;
        }

        Debug.Log("move3d == " + move3d.ToString() + " ; magnitude == " + move3d.magnitude.ToString());

        Vector3 vel = transform.forward * move.magnitude * speed * Mathf.Abs(Vector3.Dot(move3d, transform.forward)); //fix turning here!!!

        Debug.Log("vel == " + vel.ToString() + " ; speed == " + vel.magnitude.ToString());

        m_playerRB.velocity = Vector3.Lerp(m_playerRB.velocity, vel, 0.3f);

        //Debug.DrawLine(transform.position, transform.position + vel, Color.red);
        //Debug.Log("m_playerRB.velocity == " + m_playerRB.velocity.ToString() + " ; speed == " + m_playerRB.velocity.magnitude.ToString());

        if (move3d.magnitude < 0.05)
        {
            return;
        }
        else
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(move3d * ((speed >= 0.0f)?1.0f:-1.0f), transform.up).normalized, transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * Mathf.SmoothStep(0.15f, 1.0f, 1.0f - m_playerRB.velocity.magnitude / m_maxSpeed));
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_turnSpeed * Mathf.Lerp(0.1f, 1.0f, 1.0f - m_playerRB.velocity.magnitude / m_maxSpeed));
        }
    }

    private void AirMove (Vector2 move)
    {
        if (!m_playerRB.useGravity)
        {
            m_playerRB.useGravity = true;
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
