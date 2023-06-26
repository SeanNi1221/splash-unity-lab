using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateHandle : FlexibleHandle {

  private const float _rotationSpeed = 500f;
  private static float deltaX => -Input.GetAxis("Mouse X");
  private static float deltaY => Input.GetAxis("Mouse Y");

  public override void OnPointerDown(PointerEventData eventData) {
    base.OnPointerDown(eventData);
    if (Dragged == this) {
      StartCoroutine(RotateCoroutine());
    }
  }

  private IEnumerator RotateCoroutine() {
    yield return new WaitForSeconds(0.1f);
    GameManager.Selected.ViewSpaceMover.SetPlane();
    HideGroup(GizmoGroup.Frame);
    HideGroup(GizmoGroup.MoveHandle);
    while (Dragged == this) {
      GameManager.Selected.transform.Rotate(Vector3.up,
                                            deltaX * _rotationSpeed * Time.deltaTime,
                                            Space.World);
      GameManager.Selected.transform.Rotate(Vector3.right,
                                            deltaY * _rotationSpeed * Time.deltaTime,
                                            Space.World);
      yield return null;
    }
    ShowGroup(GizmoGroup.Frame);
    yield return AdaptVisibilitiesCoroutine();
  }
}
