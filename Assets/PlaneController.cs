using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlaneControlMode
{
    KeyboardPhysic,
    KeyBoard
}

public class PlaneController : MonoBehaviour
{
    [SerializeField]
    private float xAngleConstraint = 15;

    [SerializeField]
    private float zAngleConstraint = 15;

    [SerializeField]
    private float angleVelocity;

    [SerializeField] private PlaneControlMode _planeControlMode;

    [SerializeField] private Rigidbody _rigidbody;

    private float _horizontal;
    private float _vertical;

    void FixedUpdate()
    {
        if (_planeControlMode == PlaneControlMode.KeyboardPhysic)
        {
            var rotation = _rigidbody.rotation.eulerAngles;
            rotation = rotation + new Vector3(_vertical, 0, _horizontal) * angleVelocity;

            // limit rotation
            var xAngle = (rotation.x > 180) ? rotation.x - 360 : rotation.x;

            if (xAngle > xAngleConstraint)
                rotation.x = xAngleConstraint;
            else if (xAngle < xAngleConstraint * -1)
                rotation.x = xAngleConstraint * -1;

            var zAngle = (rotation.z > 180) ? rotation.z - 360 : rotation.z;

            if (zAngle > zAngleConstraint)
                rotation.z = zAngleConstraint;
            else if (zAngle < zAngleConstraint * -1)
                rotation.z = zAngleConstraint * -1;

            _rigidbody.rotation = Quaternion.Lerp( _rigidbody.rotation, Quaternion.Euler(rotation), Time.time * 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxis("Horizontal") * -1;
        _vertical = Input.GetAxis("Vertical");

        if (_planeControlMode == PlaneControlMode.KeyBoard)
        {
            var rotation = transform.eulerAngles + new Vector3(_vertical, 0, _horizontal) * angleVelocity;

            // limit rotation
            var xAngle = (rotation.x > 180) ? rotation.x - 360 : rotation.x;

            if (xAngle > xAngleConstraint)
                rotation.x = xAngleConstraint;
            else if (xAngle < xAngleConstraint * -1)
                rotation.x = xAngleConstraint * -1;

            var zAngle = (rotation.z > 180) ? rotation.z - 360 : rotation.z;

            if (zAngle > zAngleConstraint)
                rotation.z = zAngleConstraint;
            else if (zAngle < zAngleConstraint * -1)
                rotation.z = zAngleConstraint * -1;

            transform.eulerAngles = rotation;
        }
    }
}
