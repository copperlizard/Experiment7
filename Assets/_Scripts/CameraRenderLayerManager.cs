using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraRenderLayerManager : MonoBehaviour
{
    private Camera m_mainCamera;

    [SerializeField]
    private Camera m_worldCamera, m_planeIndicatorCamera;

    private RenderTexture m_renderTexture = null;

    private void OnEnable()
    {
        m_mainCamera = GetComponent<Camera>();
        //m_renderTexture = new RenderTexture(m_mainCamera.pixelWidth, m_mainCamera.pixelWidth, 24);

        if (m_worldCamera == null)
        {
            Debug.Log("m_worldCamera not assigned!");
        }

        if (m_planeIndicatorCamera == null)
        {
            Debug.Log("m_planeIndicatorCamera not assigned!");
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnPreRender()
    {
        if (m_worldCamera == null || m_planeIndicatorCamera == null)
        {
            return;
        }

        m_renderTexture = RenderTexture.GetTemporary(m_mainCamera.pixelWidth, m_mainCamera.pixelHeight);

        m_worldCamera.targetTexture = m_renderTexture;
        m_worldCamera.Render();
        m_worldCamera.targetTexture = null;

        m_planeIndicatorCamera.targetTexture = m_renderTexture;
        m_planeIndicatorCamera.Render();
        m_planeIndicatorCamera.targetTexture = null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_worldCamera == null || m_planeIndicatorCamera == null)
        {
            Graphics.Blit(source, destination);
        }            
        else
        {
            Graphics.Blit(m_renderTexture, destination); //replace camera source with composite of world and planeIndicator cameras...
            RenderTexture.ReleaseTemporary(m_renderTexture);
        }                    
    }
}
