using System;
using Obi;
using UnityEngine;
using Zenject;

public class Puddle : MonoBehaviour
{
    [SerializeField] protected ObiEmitter _emitter;
    [SerializeField] private ObiCollider _obiCollider;
    [SerializeField] private ObiEmitterShapeDisk _disk;
    [SerializeField] private ObiParticleRenderer _renderer;

    //[Inject] private Render _render;
    
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
}