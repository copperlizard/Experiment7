using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CallScrollBarScroll : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private ScrollBarScroller m_scroller;

    [SerializeField]
    private float m_tarPos = 1.0f;

    private Slider m_slider;

	// Use this for initialization
	void Start ()
    {
        if (m_scroller == null)
        {
            Debug.Log("m_scroller not assigned!");
        }

        m_slider = GetComponent<Slider>();
        if (m_slider == null)
        {
            Debug.Log("m_slider not assigned!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnSelect(BaseEventData eventData)
    {
        m_scroller.ScrollTo(m_tarPos);
    }
}
