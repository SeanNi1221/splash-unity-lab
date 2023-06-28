using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHandle3D : Handle3D
{
  private const string _angleColorRef = "_AngleColor";
  private const string _angleStartRef = "_AngleStart";
  private const string _angleRangeRef = "_AngleRange";
  private static readonly Dictionary<GizmoAnchor, Vector3> _anchorToAxis =
      new Dictionary<GizmoAnchor, Vector3>() {
          { GizmoAnchor.X, Vector3.right },
          { GizmoAnchor.Y, Vector3.up },
          { GizmoAnchor.Z, Vector3.forward },
      };

  private static readonly Dictionary<GizmoAnchor, int> _anchorToComponent =
      new Dictionary<GizmoAnchor, int>() {
          { GizmoAnchor.X, 0 },
          { GizmoAnchor.Y, 1 },
          { GizmoAnchor.Z, 2 },
      };

  private static readonly Color _angleColor = new Color(0.4980392f, 0.1137255f, 0.7490196f);
  protected override void OnMouseDown() {
    base.OnMouseDown();
    StartCoroutine(RotateAroundCoroutine(GameManager.Selected));
  }

  protected override void Awake() {
    base.Awake();
    _propertyBlock.SetColor(_angleColorRef, _angleColor);
    _renderer.SetPropertyBlock(_propertyBlock, 0);
    ResetMaterialAngles();
  }

  private IEnumerator RotateAroundCoroutine(Selectable obj) {
    int component = _anchorToComponent[Anchor];
    Vector3 center = obj.transform.position;
    // Stores initial values
    Vector3 initialPointerDirection = GetPointerOnZplane() - center;
    float startAngle = -Vector3.SignedAngle(transform.up,
                                           initialPointerDirection,
                                           transform.forward);
    SetMaterialStartAngle(startAngle);

    Vector3 oldPointerDirection = initialPointerDirection;
    float angleRange = 0f;
    while (Dragged == this) {
      // Calculates mouse angle
      Vector3 pointerDirection = GetPointerOnZplane() - center;
      float angleDelta = Vector3.SignedAngle(oldPointerDirection,
                                             pointerDirection,
                                             transform.forward);
      oldPointerDirection = pointerDirection;
      // Rotates the object.
      obj.transform.RotateAround(center, transform.forward, angleDelta);
      Debug.DrawLine(center, center + initialPointerDirection, Color.white);
      Debug.DrawLine(center, center + pointerDirection, Color.white);
      angleRange -= angleDelta;
      SetMaterialAngleRange(angleRange);
      yield return null;
    }
    ResetMaterialAngles();
  }

  private Vector3 GetPointerOnZplane() {
    Ray pointerRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    Vector3 zNormal = transform.forward;
    // zPlane: the plane that this ring-shaped handle lies on.
    Plane zPlane = new Plane(zNormal, transform.position);
    float zAngle = Vector3.Angle(pointerRay.direction, zNormal);
    if (zAngle < 85f || zAngle > 95f) {
      zPlane.Raycast(pointerRay, out float enter);
      return pointerRay.GetPoint(Mathf.Abs(enter));
    } else {

      // pointerPlane: the plane that is perpendicular to the pointerRay and
      // passes through the center.
      Plane pointerPlane = new Plane(pointerRay.direction, transform.position);
      pointerPlane.Raycast(pointerRay, out float pointerEnter);
      Vector3 pointerPoint = pointerRay.GetPoint(pointerEnter);

      Ray pointerToZ = new Ray(pointerPoint, zNormal);
      zPlane.Raycast(pointerToZ, out float zEnter);
      Vector3 upPoint = pointerToZ.GetPoint(zEnter);
      float radius = transform.lossyScale.x / 2f;
      float upDistance = Mathf.Min(Vector3.Distance(transform.position, upPoint), radius);
      float forwardDistance = Mathf.Sqrt(radius * radius - upDistance * upDistance);
      Vector3 circlePoint = upPoint + -pointerRay.direction * forwardDistance;
      return circlePoint;
    }
  }

  private void SetMaterialStartAngle(float angle) {
    _propertyBlock.SetFloat(_angleStartRef, angle);
    _renderer.SetPropertyBlock(_propertyBlock, 0);
  }

  private void SetMaterialAngleRange(float angle) {
    _propertyBlock.SetFloat(_angleRangeRef, angle);
    _renderer.SetPropertyBlock(_propertyBlock, 0);
  }

  private void ResetMaterialAngles() {
    _propertyBlock.SetFloat(_angleStartRef, 0f);
    _propertyBlock.SetFloat(_angleRangeRef, 0f);
    _renderer.SetPropertyBlock(_propertyBlock, 0);
  }

}
