using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarScroller : MonoBehaviour
{
    [SerializeField]
    private float m_lerpRate = 0.3f;

    private Scrollbar m_scrollbar;

    private bool m_stopScroll = false, m_scrolling = false;

	// Use this for initialization
	void Start ()
    {
        m_scrollbar = GetComponent<Scrollbar>();
        if (m_scrollbar == null)
        {
            Debug.Log("m_scrollbar not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ScrollTo (float tar)
    {
        if (m_scrolling)
        {
            m_stopScroll = true;
        }
        else
        {
            StartCoroutine(Scroll(tar));
        }
    }

    IEnumerator QueueScroll (float tar)
    {
        while (m_stopScroll)
        {
            yield return null;
        }

        StartCoroutine(Scroll(tar));
    }

    IEnumerator Scroll (float tar)
    {
        m_scrolling = true;        

        while (m_scrollbar.value != tar && !m_stopScroll)
        {
            m_scrollbar.value = Mathf.Lerp(m_scrollbar.value, tar, m_lerpRate);

            if (Mathf.Abs(tar - m_scrollbar.value) < 0.05f)
            {
                m_scrollbar.value = tar;
            }

            yield return null;
        }

        yield return null;
        m_scrolling = false;
        m_stopScroll = false;
    }
}
