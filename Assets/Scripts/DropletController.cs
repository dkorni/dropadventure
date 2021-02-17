using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class DropletController : MonoBehaviour
{
    [SerializeField]
    private ObiSolver _obiSolver;

    [SerializeField]
    private ObiEmitter _obiEmitter;

    [SerializeField] private Component _deathCollider;

    // Start is called before the first frame update
    void Start()
    {
        _obiSolver.OnCollision += ObiSolverOnOnCollision;
        _obiSolver.OnParticleCollision += ObiSolverOnOnParticleCollision;
        //_obiSolver.OnInterpolate += ObiSolverOnOnInterpolate;
        //_obiSolver.OnSubstep += ObiSolverOnOnSubstep;
        //_obiSolver.OnUpdateParameters += ObiSolverOnOnUpdateParameters;

       // _obiEmitter.OnKillParticle += ObiEmitterOnOnKillParticle;
    }

    private void ObiSolverOnOnParticleCollision(ObiSolver solver, ObiSolver.ObiCollisionEventArgs contacts)
    {
       // Debug.Log("Particle Collision");
    }

    private void ObiSolverOnOnCollision(ObiSolver solver, ObiSolver.ObiCollisionEventArgs contacts)
    {
        var contact = contacts.contacts[0];

        // this one is an actual collision:
        if (contact.distance < 0.01)
        {
            ObiCollider.idToCollider.TryGetValue(contact.other, out var collider);
            if (collider != null)
            {
                if (collider.name == "Plane (1)")
                {
                    Debug.Log("Death!");
                }
                // do something with the collider.
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
