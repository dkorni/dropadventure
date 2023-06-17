using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDetonationBehaviour : MonoBehaviour, IDetonationSubscriber
{
    private Camera _camera;
    private Coroutine _coroutine;

    public void OnDetonated()
    {
        if (_coroutine != null)
            return;

        _coroutine = StartCoroutine(Shaking());
        Debug.Log("Detonated!");
    }

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private IEnumerator Shaking()
    {
        float duration = 3f; 

        float normalizedTime = 0;
        var startCamSize = Camera.main.fieldOfView;
        while (normalizedTime <= 1f)
        {
            _camera.fieldOfView = Random.Range(startCamSize - 2.5f, startCamSize);


            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        _camera.fieldOfView = startCamSize;
        _coroutine = null;
    }
}
