using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Zenject;

public class Laser : MonoBehaviour
{
    public float MaxDistance = 1000;

    [Inject] GameContext gameContext;
    [SerializeField] VisualEffect visualEffect;

    public Transform smoke;

    private Vector3 position;
    private Coroutine coroutine;

    private void Start()
    {
        gameContext.OnStartLaser += StartLaser;
        gameContext.OnUpdateLaser += UpdateLasser;
        gameContext.OnStopLaser += StopLaser;
    }

    private void OnDisable()
    {
        gameContext.OnUpdateLaser -= UpdateLasser;
        gameContext.OnStopLaser -= StopLaser;
        gameContext.OnStartLaser -= StartLaser;
    }

    private void StartLaser()
    {
        smoke.gameObject.SetActive(true);
        coroutine = StartCoroutine(UpdateLaserCoroutine());
    }

    private void StopLaser()
    {
        visualEffect.enabled = false;
        StopCoroutine(coroutine);
        smoke.gameObject.SetActive(false);
    }

    private void UpdateLasser(Vector3 position)
    {
       this.position = position;
    }

    private IEnumerator UpdateLaserCoroutine()
    {
        while (true)
        {
            var ray = Camera.main.ScreenPointToRay(this.position);
            if (Physics.Raycast(ray, out var hit, MaxDistance))
            {
                smoke.transform.position = hit.point;
                visualEffect.enabled = true;
                transform.LookAt(hit.point);
                transform.position = new Vector3(hit.point.x, transform.position.y, transform.position.z);
                if (hit.transform.CompareTag("IceCream"))
                {
                    var iceCream = hit.transform.GetComponent<IceCream>();
                    iceCream.Melt();
                }
            }
               

            yield return null;
        }
    }
}