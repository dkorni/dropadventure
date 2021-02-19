using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Obi;
using UnityEngine;

public class DropletController : MonoBehaviour
{
    [SerializeField]
    private ObiSolver _obiSolver;

    [SerializeField]
    private ObiEmitter _obiEmitter;

    [SerializeField] private Component _deathCollider;

    [SerializeField] private ObiFluidRenderer _mainRenderer;

    [SerializeField] private ObiFluidRenderer _connectableRenderer;

    [SerializeField] private int _diedCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _obiSolver.OnCollision += ObiSolverOnOnCollision;
    }

    private void ObiSolverOnOnCollision(ObiSolver solver, ObiSolver.ObiCollisionEventArgs contacts)
    {
     //   var contact = contacts.contacts[0];


        foreach (var contact in contacts.contacts)
        {
            // this one is an actual collision:
            if (contact.distance < 0.01)
            {
                ObiCollider.idToCollider.TryGetValue(contact.other, out var collider);
                if (collider != null)
                {
                    if (collider.tag == "GameOverTrigger")
                    {
                       // _diedCount++;
                    }

                    if (collider.tag == "Connectable")
                    {
                        collider.GetComponent<ObiEmitter>().collisionMaterial = _obiEmitter.collisionMaterial;
                        collider.GetComponent<ObiEmitter>().emitterBlueprint = _obiEmitter.emitterBlueprint;
                        collider.transform.parent = transform;

                        var particleRenderer = collider.GetComponent<ObiParticleRenderer>();

                        var connectableParticleRendList = _connectableRenderer.particleRenderers.ToList();
                        connectableParticleRendList.Remove(particleRenderer);
                        _connectableRenderer.particleRenderers = connectableParticleRendList.ToArray();

                        var mainParticleRendList = _mainRenderer.particleRenderers.ToList();
                        mainParticleRendList.Add(particleRenderer);
                        _mainRenderer.particleRenderers = mainParticleRendList.ToArray();

                        collider.tag = "Untagged";
                    }

                    // do something with the collider.
                }
            }
        }


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
