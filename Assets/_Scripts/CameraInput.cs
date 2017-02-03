using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    private CameraController m_cameraController;

    private Vector2 m_move;

    // Use this for initialization
    void Start ()
    {
        m_cameraController = GetComponent<CameraController>();

        if (m_cameraController == null)
        {
            Debug.Log("m_cameraController not found!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        GetInput();

        m_cameraController.PanTilt(m_move);
    }

    private void GetInput()
    {
        m_move.x = Input.GetAxis("Horizontal");
        m_move.y = Input.GetAxis("Vertical");

        //Debug.Log("m_move == " + m_move.ToString());
    }
}
