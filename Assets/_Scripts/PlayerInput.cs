using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerController m_playerController;
    private Vector2 m_move;

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

        m_playerController.Move(m_move);
	}

    private void GetInput ()
    {
        m_move.x = Input.GetAxis("Horizontal");
        m_move.y = Input.GetAxis("Vertical");

        //Debug.Log("m_move == " + m_move.ToString());
    }
}
