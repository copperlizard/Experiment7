using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float m_jumpChargeRate = 5.0f;

    private PlayerController m_playerController;
    private Vector2 m_move;

    private float m_jumpCharge = 0.0f;

    private bool m_jumpCharging = false, m_jump = false;

	// Use this for initialization
	void Start ()
    {
        m_playerController = GetComponent<PlayerController>();

        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetInput();
	}

    private void FixedUpdate()
    {   
        m_playerController.Move(m_move);

        m_playerController.SetJumpCharge(m_jumpCharge);
    }

    private void GetInput ()
    {
        m_move.x = Input.GetAxis("Horizontal");
        m_move.y = Input.GetAxis("Vertical");

        if (Input.GetButton("Jump") && !m_jumpCharging && m_playerController.PlayerIsGrounded())
        {
            StartCoroutine(JumpCharge());
            m_jump = true;
        }
        else if (Input.GetButtonDown("Jump") && !m_playerController.PlayerIsGrounded())
        {
            //Debug.Log("dash!");
            m_jumpCharge = 0.0f; //maybe uneccessary, trying to fix bug where charge gets "stuck"
            m_playerController.AirDash(m_move);
        }
        else if (Input.GetButtonUp("Jump") && m_jump && m_playerController.PlayerIsGrounded())
        {
            //Debug.Log("jump!");
            m_playerController.Jump();
            m_jumpCharge = 0.0f;
        }
        else if (Input.GetButtonUp("Jump") && m_jump && !m_playerController.PlayerIsGrounded())
        {
            m_jumpCharge = 0.0f;
        }
    }

    private IEnumerator JumpCharge ()
    {
        m_jumpCharging = true;
        do
        {
            m_jumpCharge = Mathf.SmoothStep(m_jumpCharge, 1.0f, m_jumpChargeRate * Time.deltaTime);
            yield return null;
        } while (Input.GetButton("Jump"));

        m_jumpCharging = false;

        yield return null;
    }
}
