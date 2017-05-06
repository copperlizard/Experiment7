using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer m_mainMixer;

    [SerializeField]
    private Slider m_masterVolumeSlider, m_musicVolumdeSlider, m_SFXVolumeSlider, m_footStepsVolumeSlider, m_turnSensitivitySlider;
    
    // Use this for initialization
	void Start ()
    {
		if (m_mainMixer == null)
        {
            Debug.Log("m_mainMixer not assigned!");
        }

        if (m_masterVolumeSlider == null)
        {
            Debug.Log("m_masterVolumeSlider not assigned!");
        }

        if (m_musicVolumdeSlider == null)
        {
            Debug.Log("m_musicVolumdeSlider not assigned!");
        }

        if (m_SFXVolumeSlider == null)
        {
            Debug.Log("m_SFXVolumeSlider not assigned!");
        }

        if (m_footStepsVolumeSlider == null)
        {
            Debug.Log("m_footStepsVolumeSlider not assigned!");
        }

        if (m_turnSensitivitySlider == null)
        {
            Debug.Log("m_turnSensitivitySlider not assigned!");
        }
        
        m_masterVolumeSlider.value = GetMasterVolume();
        m_musicVolumdeSlider.value = GetMusicVolume();
        m_SFXVolumeSlider.value = GetSFXVolume();
        m_footStepsVolumeSlider.value = GetFootStepVolume();
        m_turnSensitivitySlider.value = GetTiltLerpRate();

        m_mainMixer.SetFloat("MasterVolume", GetMasterVolume());
        m_mainMixer.SetFloat("MusicVolume", GetMusicVolume());
        m_mainMixer.SetFloat("SFXVolume", GetSFXVolume());
        m_mainMixer.SetFloat("FootStepsVolume", GetFootStepVolume());
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetMasterVolume (float vol)
    {
        //Debug.Log("vol == " + vol.ToString());
        PlayerPrefs.SetFloat("MasterVolume", vol);
        m_mainMixer.SetFloat("MasterVolume", vol);
    }

    public void SetMusicVolume(float vol)
    {
        //Debug.Log("vol == " + vol.ToString());
        PlayerPrefs.SetFloat("MusicVolume", vol);
        m_mainMixer.SetFloat("MusicVolume", vol);
    }

    public void SetSFXVolume(float vol)
    {
        //Debug.Log("vol == " + vol.ToString());
        PlayerPrefs.SetFloat("SFXVolume", vol);
        m_mainMixer.SetFloat("SFXVolume", vol);
    }

    public void SetFootStepVolume(float vol)
    {
        //Debug.Log("vol == " + vol.ToString());
        PlayerPrefs.SetFloat("FootStepsVolume", vol);
        m_mainMixer.SetFloat("FootStepsVolume", vol);
    }

    public float GetMasterVolume()
    {
        //Debug.Log("vol == " + vol.ToString());
        return PlayerPrefs.GetFloat("MasterVolume", 0.0f);
    }

    public float GetMusicVolume()
    {
        //Debug.Log("vol == " + vol.ToString());
        return PlayerPrefs.GetFloat("MusicVolume", 0.0f);
    }

    public float GetSFXVolume()
    {
        //Debug.Log("vol == " + vol.ToString());
        return PlayerPrefs.GetFloat("SFXVolume", 0.0f);
    }

    public float GetFootStepVolume()
    {
        //Debug.Log("vol == " + vol.ToString());
        return PlayerPrefs.GetFloat("FootStepsVolume", -2.0f);
    }

    public void SetTiltLerpRate(float rate)
    {
        PlayerPrefs.SetFloat("LerpTiltRate", rate);
    }

    public float GetTiltLerpRate()
    {
        return PlayerPrefs.GetFloat("LerpTiltRate", 1.0f);
    }
}
