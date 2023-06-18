using Assets.Scripts.Interfaces;
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
    public GameObject Smashable;
    public HingeJoint HingeJoint;
    public AudioClip PuddleAppearanceAudioClip;

    private AudioSource audioSource;
    private ISmashable smashable;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        smashable = Smashable.GetComponent<ISmashable>();
        smashable.Prepare();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Platform"))
        {
            Rigidbody.isKinematic = true;
            MeshRenderer.enabled = false;
            Collider.enabled = false;
            HingeJoint.connectedBody = null;

            foreach (var piece in Pieces)
            {
                piece.gameObject.SetActive(true);
                piece.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 3.0f, ForceMode.Impulse);
            }

            smashable.Smash(collision);
            audioSource.PlayOneShot(PuddleAppearanceAudioClip);
        }
    }
}
