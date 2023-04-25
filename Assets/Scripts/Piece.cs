using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Platform") || collision.transform.CompareTag("GameOverTrigger"))
        {
            if (_particleSystem != null)
            {
                _particleSystem.Stop();
                _particleSystem.transform.parent = null;
            }
           
            gameObject.SetActive(false);
        }
    }
}
