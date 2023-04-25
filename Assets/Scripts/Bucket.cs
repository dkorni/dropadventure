using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public Collider Collider;
    public Rigidbody Rigidbody;
    public float ExplosionForce = 10.0f;
    public Rigidbody[] Pieces;
    public float ExplosionRadius = 2.0f;
    public GameObject Puddle;
    
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Platform"))
        {
            Rigidbody.isKinematic = true;
            MeshRenderer.enabled = false;
            Collider.enabled = false;
            
            foreach (var piece in Pieces)
            {
                piece.gameObject.SetActive(true);
                piece.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 3.0f, ForceMode.Impulse);
            }

            Puddle.transform.position = collision.contacts[0].point + new Vector3(0,0.52f,0);
            Puddle.SetActive(true);
        }
    }
}
