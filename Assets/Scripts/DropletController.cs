using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Extensions;
using Obi;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class DropletController : MonoBehaviour, IDetonationSubscriber
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

    public int FlushedParticles;

    public int HealthPersantage => health * 100 / MaxHealth;

    private float Speed = 1.035f;

    public float DelayTimeToStart;

    private bool isStarted;
    private float currentSpeed;
    
    private ObiEmitter[] emitters;

    [Inject]
    private DynamicJoystick joystick;

    // Start is called before the first frame update
    void Start()
    {
        emitters = FindObjectsOfType<ObiEmitter>();
        _obiSolver.OnCollision += ObiSolverOnOnCollision;
        _emitter.solver.OnBeginStep += Solver_OnBeginStep;
        var health = _emitter.GetMaxPoints();
        var maxHealth = health;

        OnDied += () =>
        {
            _emitter.solver.OnBeginStep -= Solver_OnBeginStep;
        };

       _emitter.OnKillParticle += (obiEmitter, index) =>
       {
           if (obiEmitter.activeParticleCount == 1)
               obiEmitter.isRespawnable = false;
           UpdateHealth(-1);
           FlushedParticles++;
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
               FlushedParticles++;
           };
       }

        UpdateMaxHealth(maxHealth);
        UpdateHealth(health);
        StartCoroutine(StartWithDelay());
    }

    public void OnDestroy()
    {
        _emitter.solver.OnBeginStep -= Solver_OnBeginStep;
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

                    else if (collider.tag == "DropCollider")
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

                    else if (collider.tag == "Bomb")
                    {
                       var bomb = collider.GetComponent<Bomb>();
                       bomb.Detonate();
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

    private void Solver_OnBeginStep(ObiSolver solver, float stepTime)
    {
        var indices = new NativeArray<int>(_emitter.solverIndices, Allocator.TempJob);

        var job = new CustomGravityJob
        {
            indices = indices,
            velocities = _emitter.solver.velocities.AsNativeArray<float4>(),
            speed = currentSpeed
        };

        job.Schedule(indices.Length, 128).Complete();
    }

    private IEnumerator StartWithDelay()
    {
        while(joystick.Horizontal == 0 && joystick.Vertical == 0)
        {
            yield return null;
        }
        currentSpeed = Speed;
    }

    public void OnDetonated()
    {
        _obiSolver.OnCollision -= ObiSolverOnOnCollision;
        _emitter.solver.OnBeginStep -= Solver_OnBeginStep;
        _emitter.AddForce(_emitter.transform.up * 0.01f, ForceMode.Impulse);
        StartCoroutine(KillAllAfterExplosion());
    }

    private IEnumerator KillAllAfterExplosion()
    { 
        yield return new WaitForSeconds(3.1f);
        
        OnDied?.Invoke();
    }

    [BurstCompile]
    struct CustomGravityJob : IJobParallelFor
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<int> indices;
        public NativeArray<float4> velocities;

        [ReadOnly] public float speed;
        
        public void Execute(int i)
        {
            var index = indices[i];
            var vel = velocities[index];
            vel.xyz = vel.xyz * speed;
            velocities[index] = vel;
        }
    }
}