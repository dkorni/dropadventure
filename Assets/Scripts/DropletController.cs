using System;
using System.Linq;
using Obi;
using UnityEngine;

public class DropletController : Puddle
{
    public event Action OnDied;
    public Action<int> OnMaxHealthUpdate;
    public Action<int> OnHealthUpdate;

    [SerializeField] private AudioSource _source;

    [SerializeField]
    private ObiSolver _obiSolver;

    private int health;

    private int MaxHealth;

    // Start is called before the first frame update
    void Start()
    {
       _obiSolver.OnCollision += ObiSolverOnOnCollision;
       _emitter.OnEmitParticle += (e,i) =>
       {
           UpdateHealth(1);
           UpdateMaxHealth(1);
       };
       _emitter.OnKillParticle += (obiEmitter, index) => UpdateHealth(-1); 

       var emitters = FindObjectsOfType<ObiEmitter>();
       foreach (var emitter in emitters)
       {
           if(emitter == _emitter)
               continue;
           
           emitter.OnKillParticle += (obiEmitter, index) => UpdateHealth(-1); 
           
           emitter.OnEmitParticle += (e, i) =>
           {
               UpdateMaxHealth(1);
           };
       }
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
                        puddle.Join(_emitter);
                        _source.PlayOneShot(_source.clip);
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