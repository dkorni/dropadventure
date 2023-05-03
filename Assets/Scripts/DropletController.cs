using System;
using System.Collections.Generic;
using Assets.Scripts.Extensions;
using Obi;
using UnityEngine;

public class DropletController : MonoBehaviour
{
    public event Action OnDied;
    public event Action OnJoin;
    public event Action OnWashedAway;
    public event Action OnFlush;

    public event Action<int> OnMaxHealthUpdate;
    public event Action<int> OnHealthUpdate;

    [SerializeField] protected ObiEmitter _emitter;
    [SerializeField] protected ObiParticleRenderer _renderer;
    [SerializeField] private AudioSource _source;
    [SerializeField] private ObiEmitterShapeDisk _disk;

    [SerializeField]
    private ObiSolver _obiSolver;

    public int health;

    public int MaxHealth;
    
    [SerializeField]
    private ObiEmitter[] emitters;

    private HashSet<Puddle> JoinedPuddles = new HashSet<Puddle>();
    private object lockObject = new object();

    // Start is called before the first frame update
    void Start()
    {
       _obiSolver.OnCollision += ObiSolverOnOnCollision;
        var health = _emitter.GetMaxPoints();
        var maxHealth = health;
     
       _emitter.OnKillParticle += (obiEmitter, index) =>
       {
           if (obiEmitter.activeParticleCount == 1)
               obiEmitter.isRespawnable = false;
           UpdateHealth(-1);
       }; 
       
       foreach (var emitter in emitters)
       {
           if(emitter == _emitter)
               continue;

           maxHealth += emitter.GetMaxPoints();
           emitter.OnKillParticle += (obiEmitter, index) =>
           {
               if (obiEmitter.activeParticleCount == 1)
                   obiEmitter.isRespawnable = false;
               UpdateHealth(-1);
           };
       }

        UpdateMaxHealth(maxHealth);
        UpdateHealth(health);
    }

    private void ObiSolverOnOnCollision(ObiSolver solver, ObiSolver.ObiCollisionEventArgs contacts)
    {
        foreach (var contact in contacts.contacts)
        {
            // this one is an actual collision:
            if (contact.distance < 0.01)
            {
                var world = ObiColliderWorld.GetInstance();
                if(!(world.colliderHandles.Count > contact.bodyB))
                    return;
                
                ObiColliderBase collider = world.colliderHandles[contact.bodyB].owner;
                int particleIndex = solver.simplices[contact.bodyA];
                ObiSolver.ParticleInActor pa = solver.particleToActor[particleIndex];
                ObiEmitter emitter = pa.actor as ObiEmitter;
                if (collider != null)
                {
                    if (collider.tag == "GameOverTrigger")
                    {
                        // kill particle
                        emitter.life[pa.indexInActor] = 0;
                        
                        if (IsDied)
                            OnDied?.Invoke();
                    }

                    else if (collider.tag == "Connectable")
                    {
                        var puddle = collider.GetComponent<Puddle>();

                        var activeCount = puddle.GetComponent<ObiEmitter>().activeParticleCount;
                        UpdateHealth(activeCount);
                        puddle.Join(_emitter, _renderer, _disk.particleSize);
                        _source.PlayOneShot(_source.clip);
                        OnJoin?.Invoke();
                    }

                    else if(collider.tag == "FlushTrigger")
                    {
                        emitter.life[pa.indexInActor] = 0;
                        emitter.OnKillParticle += (e, i) =>
                        {
                            OnFlush?.Invoke();

                            if(IsDied)
                                OnWashedAway?.Invoke();
                        };
                    }
                }
            }
        }
    }

    private void UpdateHealth(int healthToAdd)
    {
        health += healthToAdd;
        OnHealthUpdate?.Invoke(health);
    }
    
    private void UpdateMaxHealth(int healthToAdd)
    {
        MaxHealth += healthToAdd;
        OnMaxHealthUpdate?.Invoke(MaxHealth);
    }

    private bool IsDied => health <= 0;
}