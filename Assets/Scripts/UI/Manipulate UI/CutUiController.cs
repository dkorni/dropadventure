using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

public class CutUiController : ManipulateUiController
{
    protected override void BeginDrag(PointerEventData eventData)
    {
        gameContext.StartCut();
    }

    protected override void Drag(PointerEventData eventData)
    {
        gameContext.UpdateCut(eventData.position);
    }

    protected override void EndDrag(PointerEventData eventData)
    {
        gameContext.StopCut();
    }

    protected override void OnLevelUpdated(LevelData levelData)
    {
        gameObject.SetActive(levelData.IsCutNeeded);
    }
}
