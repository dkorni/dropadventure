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

    [SerializeField] private int _diedCount = 0;

    [SerializeField] private SolverStore _solverStore;

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
                        var emitter = (ObiEmitter)solver.particleToActor[contact.particle].actor;
                        emitter.life[solver.particleToActor[contact.particle].indexInActor] = 0;

                        if (_solverStore.IsAllPlayerPuddlesDied())
                            Debug.Log("Game Over");
                    }

                    if (collider.tag == "Connectable")
                    {
                        var puddle = collider.GetComponent<Puddle>();
                        puddle.Join(_obiEmitter);
                    }
                }
            }
        }
    }
}
