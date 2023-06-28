using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHandle3D : Handle3D
{
  private const string _angleColorRef = "_AngleColor";
  private const string _angleStartRef = "_AngleStart";
  private const string _angleRangeRef = "_AngleRange";
  private static readonly Dictionary<GizmoAnchor, int> _anchorToComponent =
      new Dictionary<GizmoAnchor, int>() {
          { GizmoAnchor.X, 0 },
          { GizmoAnchor.Y, 1 },
          { GizmoAnchor.Z, 2 },
      };
  private static readonly Color _angleColor = new Color(0.4980392f, 0.1137255f, 0.7490196f);
  protected override float _sizeOnScreen => 300;
  private static Gizmo resetButton => Gizmo.Catalog[GizmoGroup.ResetButton][GizmoAnchor.Bottom];

  protected override void OnMouseDown() {
    base.OnMouseDown();
    StartCoroutine(RotateAroundCoroutine(GameManager.Selected));
  }

  protected override void Awake() {
    base.Awake();
    _propertyBlock.SetColor(_angleColorRef, _angleColor);
    _renderer.SetPropertyBlock(_propertyBlock, 0);
    ResetMaterialAngles();
    HideGroup(GizmoGroup.RotateIndicator);
  }

  private IEnumerator RotateAroundCoroutine(Selectable obj) {
    resetButton.SetValue(obj.transform);
    ShowGroup(GizmoGroup.RotateIndicator);
    int component = _anchorToComponent[Anchor];
    Vector3 center = obj.transform.position;
    // Stores initial values
    Vector3 initialPointerDirection = GetPointerOnZplane() - center;
    float startAngle = -Vector3.SignedAngle(transform.up,
                                           initialPointerDirection,
                                           transform.forward);
    SetMaterialStartAngle(startAngle);

    // References to the indicators
    RectTransform line = Gizmo.Catalog[GizmoGroup.RotateIndicator][GizmoAnchor.Center]
        .GetComponent<RectTransform>();
    Gizmo text = Gizmo.Catalog[GizmoGroup.RotateIndicator][GizmoAnchor.TC];

    Vector3 oldPointerDirection = initialPointerDirection;
    float angleRange = 0f;
    while (Dragged == this) {
      // Calculates mouse angle
      Vector3 pointerDirection = GetPointerOnZplane() - center;
      float angleDelta = Vector3.SignedAngle(oldPointerDirection,
                                             pointerDirection,
                                             transform.forward);
      // Rotates the object.
      oldPointerDirection = pointerDirection;
      obj.transform.RotateAround(center, transform.forward, angleDelta);
      angleRange -= angleDelta;
      SetMaterialAngleRange(angleRange);
      // Manages the indicators
      line.position = Camera.main.WorldToScreenPoint(center);
      Vector3 lineToMouse = Input.mousePosition - line.position;
      line.sizeDelta = new Vector2(lineToMouse.magnitude, line.sizeDelta.y);
      float mouseAngle = Vector2.SignedAngle(Vector2.right, lineToMouse);
      line.localEulerAngles = new Vector3(0, 0, mouseAngle);
      text.transform.position = line.position;
      text.SetValue(-angleRange);
      yield return null;
    }
    ResetMaterialAngles();
    HideGroup(GizmoGroup.RotateIndicator);
    if (angleRange != 0) {
      resetButton.Show();
    }
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
