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
    GameManager.Selected.ViewSpaceMover.SetPlane();
    GuidlineRig.UpdateGrids(GameManager.Selected.Renderer.bounds);
    GuidlineRig.Show();
    while (Dragged == this) {
      GameManager.Selected.ViewSpaceMover.UpdatePosition();
      GuidlineRig.UpdatePositions(GameManager.Selected.Renderer.bounds);
      yield return null;
    }
    GuidlineRig.Hide();
  }
}
