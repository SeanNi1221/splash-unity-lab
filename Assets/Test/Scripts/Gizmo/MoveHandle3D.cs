using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveHandle3D : Handle3D {
  private static readonly Dictionary<GizmoAnchor, Vector3> _anchorToAxis =
      new Dictionary<GizmoAnchor, Vector3>() {
        { GizmoAnchor.X, Vector3.right },
        { GizmoAnchor.Y, Vector3.up },
        { GizmoAnchor.Z, Vector3.back },
      };

  protected override float _sizeOnScreen => 150;

  private static Gizmo resetButton => Gizmo.Catalog[GizmoGroup.ResetButton][GizmoAnchor.Bottom];
  protected override void OnMouseDown() {
    base.OnMouseDown();
    StartCoroutine(MoveAlongCoroutine(GameManager.Selected, _anchorToAxis[Anchor]));
  }

  private IEnumerator MoveAlongCoroutine(Selectable obj, Vector3 axis) {
    resetButton.SetValue(obj.transform);
    Vector3 initialPosition = obj.transform.position;
    Vector3 initialMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
        Input.mousePosition.x,
        Input.mousePosition.y,
        Vector3.Distance(Camera.main.transform.position, obj.transform.position)));
    while (Dragged == this) {
      Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
          Input.mousePosition.x,
          Input.mousePosition.y,
          Vector3.Distance(Camera.main.transform.position, obj.transform.position)));
      Vector3 delta = mousePosition - initialMousePosition;
      Vector3 projectedDelta = Vector3.Project(delta, axis);
      obj.transform.position = initialPosition + projectedDelta;
      yield return null;
    }
    if (obj.transform.position != initialPosition) {
      resetButton.Show();
    }
  }
}
