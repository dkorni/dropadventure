using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;

    [SerializeField] private ParticleSystem _particleSystem;
    
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Platform") || collision.transform.CompareTag("GameOverTrigger"))
        {
            if (_particleSystem != null)
            {
                _particleSystem.gameObject.SetActive(true);
                _particleSystem.transform.parent = null;
                _particleSystem.transform.position = collision.contacts[0].point + _offset;
            }
           
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
}
