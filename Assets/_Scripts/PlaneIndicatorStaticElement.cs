using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneIndicatorStaticElement : MonoBehaviour
{
    private CameraController m_cameraController;

    private GameObject m_planeIndicator;

    private Vector3 m_localOffset;

	// Use this for initialization
	void Start ()
    {
        m_cameraController = Camera.main.GetComponent<CameraController>();
        if (m_cameraController == null)
        {
            Debug.Log("m_cameraController not found!");
        }

        m_planeIndicator = transform.parent.GetChild(0).gameObject;
        if (m_planeIndicator == null)
        {
            Debug.Log("m_planeIndicator not found!");
        }        

        m_localOffset = transform.localPosition - m_planeIndicator.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.DrawLine(m_planeIndicator.transform.position, m_planeIndicator.transform.position + m_localOffset);
        transform.localPosition = m_planeIndicator.transform.localPosition + Quaternion.Inverse(m_cameraController.GetPanTiltQuaternion()) * m_localOffset;
	}
}
