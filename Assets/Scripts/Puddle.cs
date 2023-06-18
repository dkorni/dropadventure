using System;
using Assets.Scripts.Interfaces;
using Obi;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class Puddle : MonoBehaviour, ISmashable
{
    [SerializeField] protected ObiEmitter _emitter;
    [SerializeField] private ObiEmitterShapeDisk _disk;
    [SerializeField] private ObiParticleRenderer _renderer;
    
    public event Action<int> OnPuddleParticleKill;
    

    public ObiEmitter Join(ObiEmitter toOther, ObiParticleRenderer otherRenderer, float diskSize)
    {
        // setup physics
        _emitter.collisionMaterial = toOther.collisionMaterial;
        _emitter.emitterBlueprint = toOther.emitterBlueprint;
        transform.parent = toOther.transform.parent;
        
        _renderer.particleColor = otherRenderer.particleColor;
        _renderer.radiusScale = otherRenderer.radiusScale;
        _disk.color = otherRenderer.particleColor;
        tag = "Untagged";
        return _emitter;
    }

    public void Prepare()
    {
       gameObject.SetActive(false);
    }

    public void Smash(Collision collision)
    {
        transform.position = collision.contacts[0].point + new Vector3(0, 0.52f, 0);
        gameObject.SetActive(true);
    }
}