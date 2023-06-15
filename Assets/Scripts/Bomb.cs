using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Animator animator;
    public GameObject paricles;
    public Action OnDetonated;

    public void Detonate()
    {
        animator.SetTrigger("Bomb");
        paricles.SetActive(true);
    }
}
