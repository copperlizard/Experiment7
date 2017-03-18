using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectOnLayer : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_projectOn;

    private Projector m_projector;

    private Material m_projectorMat;

	// Use this for initialization
	void Awake ()
    {
        m_projector = GetComponent<Projector>();
        if (m_projector == null)
        {
            Debug.Log("m_projector not found!");
        }

        m_projectorMat = new Material(m_projector.material); //create unique mat for each projector
        m_projector.material = m_projectorMat;

        m_projector.ignoreLayers = ~m_projectOn;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetProjectorColor (Color col)
    {
        m_projectorMat.color = col;
    }

    public Color GetProjectorColor()
    {
        return m_projectorMat.color;
    }

    public void SetProjectorSize (float orthoSize)
    {
        m_projector.orthographicSize = orthoSize;
    }

    public float GetProjectorSize()
    {
        return m_projector.orthographicSize;
    }
}
