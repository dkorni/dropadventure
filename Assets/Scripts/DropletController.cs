using System;
using System.Linq;
using Obi;
using UnityEngine;

public class DropletController : MonoBehaviour
{
    public event Action OnDied;
    public event Action OnJoin;
    public event Action OnWashedAway;
    public Action<int> OnMaxHealthUpdate;
    public Action<int> OnHealthUpdate;
    public Action<Vector3> OnFlush;

    [SerializeField] protected ObiEmitter _emitter;
    [SerializeField] protected ObiParticleRenderer _renderer;
    [SerializeField] private AudioSource _source;
    [SerializeField] private ObiEmitterShapeDisk _disk;

    [SerializeField]
    private ObiSolver _obiSolver;

    private int health;

    public int MaxHealth;

    private bool[] diedParticles;

    // Start is called before the first frame update
    void Start()
    {
       _obiSolver.OnCollision += ObiSolverOnOnCollision;
        var health = _emitter.GetDistributionPointsCount();
        var maxHealth = health;
     
       _emitter.OnKillParticle += (obiEmitter, index) => UpdateHealth(-1); 

       var emitters = FindObjectsOfType<ObiEmitter>();
       foreach (var emitter in emitters)
       {
           if(emitter == _emitter)
               continue;

           maxHealth += emitter.GetDistributionPointsCount();
           emitter.OnKillParticle += (obiEmitter, index) => UpdateHealth(-1);
       }

        UpdateMaxHealth(maxHealth);
        UpdateHealth(health);
        diedParticles = new bool[maxHealth+1];
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

                    if (collider.tag == "Connectable")
                    {
                        var puddle = collider.GetComponent<Puddle>();
                        var activeCount = puddle.GetComponent<ObiEmitter>().activeParticleCount;
                        UpdateHealth(activeCount);
                        puddle.Join(_emitter, _renderer, _disk.particleSize);
                        _source.PlayOneShot(_source.clip);
                        OnJoin?.Invoke();
                    }

                    if(collider.tag == "FlushTrigger")
                    {
                        emitter.life[pa.indexInActor] = 0;

                        emitter.OnKillParticle += (e, i) =>
                        {
                            if (i == particleIndex && !diedParticles[particleIndex])
                            {
                                diedParticles[particleIndex] = true;
                                OnFlush?.Invoke(contact.pointA);
                            }
                        };

                        if (IsDied)
                            OnWashedAway?.Invoke();
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