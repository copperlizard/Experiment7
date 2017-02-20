using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float m_jumpChargeRate = 5.0f, m_backConfirmTime = 1.0f;

    private GameManager m_gameManager;

    private PlayerController m_playerController;
    private Vector2 m_move;

    private float m_jumpCharge = 0.0f;

    private bool m_jumpCharging = false, m_jump = false, m_pauseJumpLock = false, m_pauseToggle = false, m_confirmingBack = false;

	// Use this for initialization
	void Start ()
    {
        m_playerController = GetComponent<PlayerController>();

        if (m_playerController == null)
        {
            Debug.Log("m_playerController not found!");
        }

        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        if (m_gameManager == null)
        {
            Debug.Log("m_gameManager not found!");
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
        if (m_gameManager.IsPaused()) //keep player from jumping after pause screen
        {           
            m_pauseJumpLock = true;
            //Debug.Log("jump locked!");            
        }
        else if (m_pauseJumpLock)
        {
            if (Input.GetButtonUp("Jump"))
            {
                m_pauseJumpLock = false;
                //Debug.Log("jump released!");
            }
        }

        if (Input.GetButtonDown("Back") && !m_confirmingBack)
        {
            StartCoroutine(ConfirmBack());
        }

        if (Input.GetButtonDown("Pause") && !m_pauseToggle)
        {            
            m_gameManager.PauseGame();
            m_pauseToggle = true;
        }
        else if (m_pauseToggle && !Input.GetButton("Pause"))
        {
            m_pauseToggle = false;
        }

        m_move.x = Input.GetAxis("Horizontal");
        m_move.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && !m_jumpCharging && m_playerController.PlayerIsGrounded() && !m_pauseJumpLock)
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

        m_playerController.Shield(Input.GetButton("Shield"), m_move);
    }

    private IEnumerator ConfirmBack ()
    {
        float startTime = Time.time;
        m_confirmingBack = true;
        bool confirmed = true;

        do
        {
            if (!Input.GetButton("Back"))
            {
                confirmed = false;
            }

            yield return null;
        } while (Time.time < startTime + m_backConfirmTime && confirmed);

        m_confirmingBack = false;

        if (confirmed)
        {
            m_gameManager.ReloadScene();
        }

        yield return null;
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
