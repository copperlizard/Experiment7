using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    virtual public void Destruct ()
    {
        Destroy(gameObject);
    }	
}
