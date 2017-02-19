using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPanelInput : MonoBehaviour
{
    private Animator m_panelAnimator;

    private Selectable m_selected;

    private bool m_panelInputActive = false, m_hold = false;

	// Use this for initialization
	void Start ()
    {
        m_panelAnimator = GetComponent<Animator>();
        if (m_panelAnimator == null)
        {
            Debug.Log("m_panelAnimator not found!");
        }

        m_selected = GetComponentInChildren<Selectable>();

        if (m_selected == null)
        {
            Debug.Log("no selectables found on panel!");
        }

        //Debug.Log("m_selected.name == " + m_selected.name);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_panelAnimator.GetBool("visible"))
        {
            GetInput();
        }
        else if (m_panelInputActive)
        {
            m_panelInputActive = false;
            m_selected = GetComponentInChildren<Selectable>();

            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
    }

    void GetInput ()
    {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
                
        if (!m_panelInputActive && move.magnitude != 0.0f)
        {
            m_panelInputActive = true;
            m_selected.Select();            
        }   
    }

    public void ResetPanelSelection ()
    {
        m_panelInputActive = false;
        m_selected = GetComponentInChildren<Selectable>();
        m_selected.Select();
    }
}
