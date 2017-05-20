using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpeedBlur : MonoBehaviour
{
    public Shader m_shader;

    private Material m_material;

    private float m_blur = 0.0f;

    // Use this for initialization
    void Start ()
    {
        if (m_material == null)
        {
            m_material = new Material(m_shader);
            m_material.hideFlags = HideFlags.HideAndDontSave;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        if (m_blur <= 0.0f)
        {
            return;
        }
        
        Graphics.Blit(source, destination, m_material);
    }

    public void SetBlurStrength (float blur)
    {
        m_blur = blur;

        m_material.SetFloat("_BlurStrength", m_blur);
    }
}
