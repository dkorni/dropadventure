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
    [SerializeField] private AudioSource _source;

    [SerializeField]
    private ObiSolver _obiSolver;

    private int health;

    public int MaxHealth;

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
    }

    private void ObiSolverOnOnCollision(ObiSolver solver, ObiSolver.ObiCollisionEventArgs contacts)
    {
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
                        // kill particle
                        var emitter = (ObiEmitter)solver.particleToActor[contact.particle].actor;
                        emitter.life[solver.particleToActor[contact.particle].indexInActor] = 0;
                        
                        if (IsDied)
                            OnDied?.Invoke();
                    }

                    if (collider.tag == "Connectable")
                    {
                        var puddle = collider.GetComponent<Puddle>();
                        var activeCount = puddle.GetComponent<ObiEmitter>().activeParticleCount;
                        UpdateHealth(activeCount);
                        puddle.Join(_emitter);
                        _source.PlayOneShot(_source.clip);
                        OnJoin?.Invoke();
                    }

                    if(collider.tag == "FlushTrigger")
                    {
                        var emitter = (ObiEmitter)solver.particleToActor[contact.particle].actor;
                        if (emitter.life[solver.particleToActor[contact.particle].indexInActor] == 0)
                            continue;
                       
                        emitter.life[solver.particleToActor[contact.particle].indexInActor] = 0;

                        emitter.OnKillParticle += (e, i) =>
                        {
                            if(i == solver.particleToActor[contact.particle].indexInActor)
                                OnFlush?.Invoke(contact.point);
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