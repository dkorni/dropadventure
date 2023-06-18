using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FromToMover : MonoBehaviour
{
    public Transform From;
    public Transform To;
    public float Speed = 1.5f;

    private Transform targetFrom;
    private Transform targetTo;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetFrom = From;
        targetTo = To;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var direction = targetTo.position - targetFrom.position;
        rb.velocity = direction * Speed;
        if(Vector3.Distance(transform.position, targetTo.position) <= 0.1f)
        {
            if(targetTo == To)
            {
                targetTo = From;
                targetFrom = To;
            }
            else
            {
                targetTo = To;
                targetFrom = From;
            }
        }
    }
}
