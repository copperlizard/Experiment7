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

    private string m_controllerType = "";

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

        if (Input.GetJoystickNames().Length > 0)
        {
            m_controllerType = Input.GetJoystickNames()[0];

            Debug.Log(m_controllerType);

            if (m_controllerType == "Wireless Controller" || m_controllerType == "")
            {
                Debug.Log("playstation layout!");
            }
            else
            {
                Debug.Log("xbox layout!");
            }
        }
        else
        {
            m_controllerType = "XBox";
        }

        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_controllerType == "Wireless Controller" || m_controllerType == "")
        {
            GetPS4Input();
        }   
        else
        {
            GetXBoxInput(); 
        }        
    }

    private void FixedUpdate()
    {   
        m_playerController.Move(m_move);

        m_playerController.SetJumpCharge(m_jumpCharge);
    }

    private void GetPS4Input()
    {
        if (m_gameManager.IsPaused()) //keep player from jumping after pause screen
        {
            m_pauseJumpLock = true;
            //Debug.Log("jump locked!");            
        }
        else if (m_pauseJumpLock)
        {
            if (Input.GetButtonUp("Button1"))
            {
                m_pauseJumpLock = false;
                //Debug.Log("jump released!");
            }
        }

        if (Input.GetButtonDown("Button8") && !m_confirmingBack)
        {
            StartCoroutine(ConfirmBack());
        }

        if (Input.GetButtonDown("Button9") && !m_pauseToggle)
        {
            m_gameManager.PauseGame();
            m_pauseToggle = true;
        }
        else if (m_pauseToggle && !Input.GetButton("Button9"))
        {
            m_pauseToggle = false;
        }

        m_move.x = Input.GetAxis("Horizontal");
        m_move.y = Input.GetAxis("Vertical");

        float sideStep = Input.GetAxis("SideStep");
        if (sideStep > 0.1f || sideStep < -0.1f)
        {
            m_playerController.SideStep(sideStep);
        }
        else if (Input.GetButtonDown("Button2"))
        {
            m_playerController.Kick();
        }
        else if (Input.GetButtonDown("Button1") && !m_jumpCharging && m_playerController.PlayerIsGrounded() && !m_pauseJumpLock && !Input.GetButton("Button0"))
        {
            StartCoroutine(JumpCharge());
            m_jump = true;
        }
        else if (Input.GetButtonDown("Button1") && !m_playerController.PlayerIsGrounded())
        {
            //Debug.Log("dash!");
            m_jumpCharge = 0.0f; //maybe uneccessary, trying to fix bug where charge gets "stuck"
            m_playerController.AirDash(m_move);
        }
        else if (Input.GetButtonUp("Button1") && m_jump && m_playerController.PlayerIsGrounded())
        {
            //Debug.Log("jump!");
            m_playerController.Jump();
            m_jumpCharge = 0.0f;
        }
        else if (Input.GetButtonUp("Button1") && m_jump && !m_playerController.PlayerIsGrounded())
        {
            m_jumpCharge = 0.0f;
        }
        else if (!m_playerController.PlayerIsShielded() && Input.GetButton("Button0") && !m_gameManager.IsPaused())
        {
            m_playerController.Shield(true);
        }
        else if (m_playerController.PlayerIsShielded() && !Input.GetButton("Button0") && !m_gameManager.IsPaused())
        {
            m_playerController.Shield(false);
        }

        m_playerController.Sprint(Input.GetButton("Button3"));
    }

    private void GetXBoxInput ()
    {
        if (m_gameManager.IsPaused()) //keep player from jumping after pause screen
        {           
            m_pauseJumpLock = true;
            //Debug.Log("jump locked!");            
        }
        else if (m_pauseJumpLock)
        {
            if (Input.GetButtonUp("Button0"))
            {
                m_pauseJumpLock = false;
                //Debug.Log("jump released!");
            }
        }

        if (Input.GetButtonDown("Button6") && !m_confirmingBack)
        {
            StartCoroutine(ConfirmBack());
        }

        if (Input.GetButtonDown("Button7") && !m_pauseToggle)
        {            
            m_gameManager.PauseGame();
            m_pauseToggle = true;
        }
        else if (m_pauseToggle && !Input.GetButton("Button7"))
        {
            m_pauseToggle = false;
        }

        m_move.x = Input.GetAxis("Horizontal");
        m_move.y = Input.GetAxis("Vertical");

        float sideStep = Input.GetAxis("SideStep");
        if (sideStep > 0.1f || sideStep < -0.1f)
        {
            m_playerController.SideStep(sideStep);
        }
        else if (Input.GetButtonDown("Button1"))
        {
            m_playerController.Kick();
        }
        else if (Input.GetButtonDown("Button0") && !m_jumpCharging && m_playerController.PlayerIsGrounded() && !m_pauseJumpLock && !Input.GetButton("Button2"))
        {
            StartCoroutine(JumpCharge());
            m_jump = true;
        }
        else if (Input.GetButtonDown("Button0") && !m_playerController.PlayerIsGrounded())
        {
            //Debug.Log("dash!");
            m_jumpCharge = 0.0f; //maybe uneccessary, trying to fix bug where charge gets "stuck"
            m_playerController.AirDash(m_move);
        }
        else if (Input.GetButtonUp("Button0") && m_jump && m_playerController.PlayerIsGrounded())
        {
            //Debug.Log("jump!");
            m_playerController.Jump();
            m_jumpCharge = 0.0f;
        }
        else if (Input.GetButtonUp("Button0") && m_jump && !m_playerController.PlayerIsGrounded())
        {
            m_jumpCharge = 0.0f;
        }
        else if (!m_playerController.PlayerIsShielded() && Input.GetButton("Button2") && !m_gameManager.IsPaused())
        {
            m_playerController.Shield(true);
        }
        else if (m_playerController.PlayerIsShielded() && !Input.GetButton("Button2") && !m_gameManager.IsPaused())
        {
            m_playerController.Shield(false);
        }

        m_playerController.Sprint(Input.GetButton("Button3"));
    }

    private IEnumerator ConfirmBack ()
    {
        float startTime = Time.time;
        m_confirmingBack = true;
        bool confirmed = true;

        do
        {
            if ((m_controllerType == "Wireless Controller" || m_controllerType == "") ? !Input.GetButton("Button8") : !Input.GetButton("Button6"))
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
        } while ((m_controllerType == "Wireless Controller" || m_controllerType == "") ? Input.GetButton("Button1") : Input.GetButton("Button0"));

        m_jumpCharging = false;

        yield return null;
    }
}
