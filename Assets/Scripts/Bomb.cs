using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Animator animator;
    
    public event Action OnDetonated;

    public float ExplosionForce = 10.0f;
    public Rigidbody[] Pieces;
    public float ExplosionRadius = 2.0f;

    public void Detonate()
    {
        animator.SetTrigger("Bomb");
        StartCoroutine(OnDetonate());
    }

    public IEnumerator OnDetonate()
    {
        yield return new WaitForSeconds(0.7f);
        foreach (var piece in Pieces)
        {
            piece.gameObject.SetActive(true);
            piece.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 1, ForceMode.Impulse);
        }
        OnDetonated?.Invoke();
    }
}