using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class CutUIController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Inject] GameContext gameContext;
    
    private Vector3 startPosition;
    private Coroutine Coroutine;

    private void OnLevelUpdated(LevelData levelData)
    {
        gameObject.SetActive(levelData.IsCutNeeded);
    }

    private void OnDisable()
    {
        gameContext.OnLevelUpdated -= OnLevelUpdated;
    }

    private void Start()
    {
        startPosition = transform.position;
        gameContext.OnLevelUpdated += OnLevelUpdated;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(Coroutine != null)
        {
            StopCoroutine(Coroutine);
        }
        gameContext.StartCut();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        gameContext.UpdateCut(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gameContext.StopCut();
        Coroutine = StartCoroutine(Return());
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
