using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (transform.localScale.x < 0.95f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.localScale.x, LayerMask.GetMask("Projectile"));

            for (int i = 0; i < hitColliders.Length; i++)
            {
                Destructible destroyable = (Destructible)hitColliders[i].gameObject.GetComponent<SlowBall>();

                if (destroyable != null)
                {
                    destroyable.Destruct();
                }
            }
        }
	}

    private void OnTriggerStay(Collider other)
    {
        Destructible destroyable = (Destructible)other.GetComponent<SlowBall>();

        if (destroyable != null)
        {
            destroyable.Destruct();
        }
    }
}
