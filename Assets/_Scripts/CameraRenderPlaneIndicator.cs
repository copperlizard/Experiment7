using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraRenderPlaneIndicator : MonoBehaviour
{
    private Camera m_mainCamera;

    [SerializeField]
    private Camera m_planeIndicatorCamera;

    private RenderTexture m_renderTexture;
    
    private void Awake()
    {
        m_mainCamera = GetComponent<Camera>();
        if (m_mainCamera == null)
        {
            Debug.Log("m_mainCamera not found!");
        }
        else
        {
            // Add a command buffer to blit the result after image effects back into the camera's render target.
            CommandBuffer commandBuffer = new CommandBuffer();
            commandBuffer.name = "MultiCameraImageEffectFix";

            commandBuffer.Blit(m_renderTexture as Texture, BuiltinRenderTextureType.CameraTarget);

            m_mainCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
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

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        m_renderTexture = src;

        m_planeIndicatorCamera.targetTexture = m_renderTexture;
        m_planeIndicatorCamera.Render();
        m_planeIndicatorCamera.targetTexture = null;

        Graphics.Blit(m_renderTexture, dest);        
    }
}
