using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blade : MonoBehaviour
{
    private bool isCutting;
    public Camera Camera;
    public float MaxDistance = 1000;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 0)
            return;
        
        var touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            // Debug.Log("Touch began");
            StartCutting();
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            // Debug.Log("Touch ended");
            StopCutting();
        }

        if (isCutting)
        {
            UpdateCutting(touch.position);
        }
    }

    private void StartCutting()
    {
        isCutting = true;
    }

    private void StopCutting()
    {
        isCutting = false;
    }

    private void UpdateCutting(Vector3 position)
    {
        var ray = Camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out var hit, MaxDistance))
        {
            if (hit.transform.CompareTag("Chain"))
            {
                var joint = hit.transform.GetComponent<HingeJoint>();
                if (joint != null)
                {
                    Destroy(joint);   
                }
            }
        }
    }
}
