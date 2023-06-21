using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveHandle : FlexibleHandle {
  public override void OnPointerDown(PointerEventData eventData) {
    base.OnPointerDown(eventData);
    if (Dragged == this) {
      StartCoroutine(MoveCoroutine());
    }
  }

  private IEnumerator MoveCoroutine() {
    _gameManager.Selected.Drag.SetPlane();
    _gameManager.GuidlineRig.UpdateGrids(_gameManager.Selected.Renderer.bounds);
    _gameManager.GuidlineRig.gameObject.SetActive(true);
    while (Dragged == this) {
      _gameManager.Selected.Drag.UpdatePosition();
      _gameManager.GuidlineRig.UpdatePositions(_gameManager.Selected.Renderer.bounds);
      yield return null;
    }
    _gameManager.GuidlineRig.gameObject.SetActive(false);
  }
}
