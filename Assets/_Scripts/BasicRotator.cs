﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_rotation;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
              
    }

    private void FixedUpdate()
    {
        transform.Rotate(m_rotation);
    }
}
