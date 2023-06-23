using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveHandle3D : Handle3D {
  private static readonly Dictionary<GizmoAnchor, UnityAction> _doMove =
      new Dictionary<GizmoAnchor, UnityAction>() {
        { GizmoAnchor.X, () => GameManager.Selected.transform.position += Vector3.right * Time.deltaTime },
        { GizmoAnchor.Y, () => GameManager.Selected.transform.position += Vector3.up * Time.deltaTime },
        { GizmoAnchor.Z, () => GameManager.Selected.transform.position += Vector3.forward * Time.deltaTime },
      };
}
