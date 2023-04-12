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

    public void Join(ObiEmitter toOther)
    {
        // setup physics
        _emitter.collisionMaterial = toOther.collisionMaterial;
        _emitter.emitterBlueprint = toOther.emitterBlueprint;
        transform.parent = toOther.transform.parent;
        _obiCollider.enabled = false;

        // update renderer
        //_render.UpdateRender(_renderer);

        tag = "Untagged";
    }
}