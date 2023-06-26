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

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        StartCoroutine(RotateAlongCoroutine(GameManager.Selected, _anchorToAxis[Anchor]));
    }

    private IEnumerator RotateAlongCoroutine(Selectable obj, Vector3 axis)
    {
        Vector3 initialPosition = obj.transform.position;
        Vector3 initialMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Vector3.Distance(Camera.main.transform.position, obj.transform.position)));
        while (Dragged == this)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Vector3.Distance(Camera.main.transform.position, obj.transform.position)));
            Vector3 delta = mousePosition - initialMousePosition;
            Vector3 projectedDelta = Vector3.Project(delta, axis);
            obj.transform.RotateAround(initialPosition, axis, projectedDelta.magnitude);
            yield return null;
        }
    }
}
