using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueRotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_rotationForce = Vector3.zero;

    private Rigidbody m_rigidBody;

	// Use this for initialization
	void Start ()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        if (m_rigidBody == null)
        {
            Debug.Log("m_rigidBody not found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void FixedUpdate()
    {
        m_rigidBody.AddTorque(m_rotationForce, ForceMode.Impulse);
    }
}
