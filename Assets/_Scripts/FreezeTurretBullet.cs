using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTurretBullet : Destructible
{
    private bool m_destructed = false;

    public override void Destruct()
    {
        if (m_destructed)
        {
            return;
        }

        m_destructed = true;        
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
