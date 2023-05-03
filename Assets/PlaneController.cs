using Assets.Scripts.ScriptableObjects;
using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlaneController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [Inject] private PlaneSO planeSO;
    [Inject] private DynamicJoystick dynamicJoystick;

    private float _horizontal;
    private float _vertical;

    void FixedUpdate()
    {
        var rotation = _rigidbody.rotation.eulerAngles;
        rotation = rotation + new Vector3(_vertical, 0, _horizontal) * planeSO.AngleVelocity;

        // limit rotation
        var xAngle = (rotation.x > 180) ? rotation.x - 360 : rotation.x;

        if (xAngle > planeSO.XAngleConstraint)
            rotation.x = planeSO.XAngleConstraint;
        else if (xAngle < planeSO.XAngleConstraint * -1)
            rotation.x = planeSO.XAngleConstraint * -1;

        var zAngle = (rotation.z > 180) ? rotation.z - 360 : rotation.z;

        if (zAngle > planeSO.ZAngleConstraint)
            rotation.z = planeSO.ZAngleConstraint;
        else if (zAngle < planeSO.ZAngleConstraint * -1)
            rotation.z = planeSO.ZAngleConstraint * -1;

        _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, Quaternion.Euler(rotation), Time.time * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = dynamicJoystick.Horizontal * -1;
        _vertical = dynamicJoystick.Vertical;
    }
}
