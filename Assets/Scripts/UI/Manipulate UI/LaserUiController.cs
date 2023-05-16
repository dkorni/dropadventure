using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LaserUiController : ManipulateUiController
{
    [SerializeField] Sprite idleIcon;
    [SerializeField] Sprite activeIcon;
    [SerializeField] Image image;

    protected override void BeginDrag(PointerEventData eventData)
    {
       image.sprite = activeIcon;
       gameContext.StartLaser();
    }

    protected override void Drag(PointerEventData eventData)
    {
        gameContext.UpdateLaser(eventData.position);
    }

    protected override void EndDrag(PointerEventData eventData)
    {
        image.sprite = idleIcon;
        gameContext.StopLaser();
    }

    protected override void OnLevelUpdated(LevelData levelData)
    {
       // todo disabling logic on start
    }
}
