using Obi;
using UnityEngine;
using Zenject;

public class Puddle : MonoBehaviour
{
    [SerializeField] private ObiEmitter _emitter;
    [SerializeField] private ObiCollider _obiCollider;
    [SerializeField] private ObiEmitterShapeDisk _disk;
    [SerializeField] private ObiParticleRenderer _renderer;

    [Inject] private SolverStore _solverStore;
    [Inject] private Render _render;

    public void Join(ObiEmitter toOther)
    {
        // setup physics
        _emitter.collisionMaterial = toOther.collisionMaterial;
        _emitter.emitterBlueprint = toOther.emitterBlueprint;
        transform.parent = toOther.transform.parent;
        _obiCollider.enabled = false;
       // _disk.enabled = false;

        // update player particle count
        _solverStore.UpdatePlayerPuddles(_emitter);

        // update renderer
        _render.UpdateRender(_renderer);

        tag = "Untagged";
    }
}
