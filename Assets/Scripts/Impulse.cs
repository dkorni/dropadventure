using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impulse : MonoBehaviour
{
    public float Velocity = 10f;

    public Rigidbody Rb;
    
    // Start is called before the first frame update
    void Start()
    {
        Rb.AddForce(Vector3.forward * Velocity, ForceMode.Impulse);
    }
}
