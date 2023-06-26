using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHandle3D : Handle3D
{
  private static readonly Dictionary<GizmoAnchor, Vector3> _anchorToAxis =
      new Dictionary<GizmoAnchor, Vector3>() {
          { GizmoAnchor.X, Vector3.right },
          { GizmoAnchor.Y, Vector3.up },
          { GizmoAnchor.Z, Vector3.back },
      };

  protected override void OnMouseDown() {
    base.OnMouseDown();
    StartCoroutine(RotateAroundCoroutine(GameManager.Selected, _anchorToAxis[Anchor]));
  }

  private IEnumerator RotateAroundCoroutine(Selectable obj, Vector3 axis) {
      Vector3 initialPosition = obj.transform.position;
      Vector3 initialMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
          Input.mousePosition.x,
          Input.mousePosition.y,
          Vector3.Distance(Camera.main.transform.position, obj.transform.position)));
      Vector3 initialMouseDirection = initialMousePosition - initialPosition;
      Quaternion initialRotation = obj.transform.rotation;

      while (Dragged == this) {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Vector3.Distance(Camera.main.transform.position, obj.transform.position)));
        Vector3 mouseDirection = mousePosition - initialPosition;
        float mouseAngle = Vector3.SignedAngle(initialMouseDirection, mouseDirection, axis);
        Quaternion rotation = Quaternion.AngleAxis(mouseAngle, axis);
        obj.transform.rotation = initialRotation * rotation;
        yield return null;
      }
  }
}
