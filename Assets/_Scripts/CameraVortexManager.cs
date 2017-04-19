using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVortexManager : MonoBehaviour
{
    private UnityStandardAssets.ImageEffects.Vortex m_camVortex;

    [SerializeField]
    private float m_vortexAngle = 50.0f;

    private float m_vortexTime = 0.0f;

	// Use this for initialization
	void Start ()
    {
        m_camVortex = GetComponent<UnityStandardAssets.ImageEffects.Vortex>();
        if (m_camVortex == null)
        {
            Debug.Log("m_camVortex not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (m_vortexTime > 0.0f)
        {
            if (!m_camVortex.enabled)
            {
                m_camVortex.enabled = true;
            }

            m_vortexTime -= Time.deltaTime;

            m_camVortex.angle = Mathf.Lerp(m_camVortex.angle, m_vortexAngle * Mathf.Cos(Time.time), 3.0f * Time.deltaTime);
        }
        else
        {
            if (m_vortexTime < 0.0f)
            {
                m_vortexTime = 0.0f;                                
            }

            if (m_camVortex.angle > 0.1f || m_camVortex.angle < -0.1f)
            {
                m_camVortex.angle = Mathf.Lerp(m_camVortex.angle, 0.0f, 3.0f * Time.deltaTime);
            }
            else
            {
                m_camVortex.angle = 0.0f;
                m_camVortex.enabled = false;
            }
        }
	}

    public void AddTime (float time)
    {
        m_vortexTime += time;

        if (m_vortexTime > 30.0f)
        {
            m_vortexTime = 30.0f;
        }
    }
}
