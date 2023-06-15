using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float AngularSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, AngularSpeed, 0));
    }
}
