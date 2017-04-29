using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    private CameraController m_cameraController;

    private string m_controllerType = "";

    private Vector2 m_move;

    // Use this for initialization
    void Start ()
    {
        m_cameraController = GetComponent<CameraController>();

        if (m_cameraController == null)
        {
            Debug.Log("m_cameraController not found!");
        }

        if (Input.GetJoystickNames().Length > 0)
        {
            m_controllerType = Input.GetJoystickNames()[0];
            if (m_controllerType == "Wireless Controller" || m_controllerType == "")
            {
                Debug.Log("(camera) playstation layout!");
            }
            else
            {
                Debug.Log("(camera) xbox layout!");
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

        m_cameraController.PanTilt(m_move);
    }

    private void GetPS4Input()
    {
        m_move.x = Input.GetAxis("3rdAxis");
        m_move.y = Input.GetAxis("6thAxis");
    }

    private void GetXBoxInput()
    {
        m_move.x = Input.GetAxis("4thAxis");
        m_move.y = Input.GetAxis("5thAxis");

        //Debug.Log("m_move == " + m_move.ToString());
    }
}
