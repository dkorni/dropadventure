using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public abstract class ManipulateUiController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Inject] protected GameContext gameContext;
    
    private Vector3 startPosition;
    private Coroutine Coroutine;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(Coroutine != null)
        {
            StopCoroutine(Coroutine);
        }
        BeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        Drag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Coroutine = StartCoroutine(Return());
        EndDrag(eventData);
    }

    protected abstract void BeginDrag(PointerEventData eventData);

    protected abstract void Drag(PointerEventData eventData);

    protected abstract void EndDrag(PointerEventData eventData);

    protected abstract void OnLevelUpdated(LevelData levelData);


    private void Start()
    {
        startPosition = transform.position;
        gameContext.OnLevelUpdated += OnLevelUpdated;
    }

    private IEnumerator Return()
    {
        while(transform.position != startPosition)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * 5);
            yield return null;
        }
    }
}
