using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int _amount = 1;

    [SerializeField]
    private float _jumpVelocity = 5;

    [SerializeField]
    private float _timeToDestroy = 3.5f;

    [SerializeField] private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody.AddForce(Vector3.up * _jumpVelocity, ForceMode.Impulse);
        Destroy(gameObject, _timeToDestroy);
    }
}