using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [SerializeField] private Vector3 v;

    [SerializeField]
    private float xAngleConstraint = 15;

    [SerializeField]
    private float zAngleConstraint = 15;

    [SerializeField]
    private float angleVelocity;

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var rotation = transform.eulerAngles + new Vector3(vertical, 0, horizontal) * angleVelocity;

        // limit rotation
        var xAngle = (rotation.x > 180) ? rotation.x - 360 : rotation.x;

        if (xAngle > xAngleConstraint)
            rotation.x = xAngleConstraint;
        else if(xAngle < xAngleConstraint * -1)
            rotation.x = xAngleConstraint * -1;

        var zAngle = (rotation.z > 180) ? rotation.z - 360 : rotation.z;

        if (zAngle > zAngleConstraint)
            rotation.z = zAngleConstraint;
        else if (zAngle < zAngleConstraint * -1)
            rotation.z = zAngleConstraint * -1;

        transform.eulerAngles = rotation;

        v = rotation;
    }
}
