using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer m_mainMixer;

    // Use this for initialization
	void Start ()
    {
		if (m_mainMixer == null)
        {
            Debug.Log("m_mainMixer not assigned!");
        }

        m_mainMixer.SetFloat("MasterVolume", GetMasterVolume());
        m_mainMixer.SetFloat("MusicVolume", GetMusicVolume());
        m_mainMixer.SetFloat("SFXVolume", GetSFXVolume());
        m_mainMixer.SetFloat("FootStepVolume", GetFootStepVolume());
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
        PlayerPrefs.SetFloat("FootStepVolume", vol);
        m_mainMixer.SetFloat("FootStepVolume", vol);
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
        return PlayerPrefs.GetFloat("FootStepVolume", -2.0f);
    }
}
