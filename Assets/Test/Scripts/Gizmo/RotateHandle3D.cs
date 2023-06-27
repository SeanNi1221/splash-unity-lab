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

  private static readonly Color angleColor = Color.magenta;
  protected override void OnMouseDown() {
    base.OnMouseDown();
    StartCoroutine(RotateAroundCoroutine(GameManager.Selected));
  }

  protected override void Awake() {
    base.Awake();
    _propertyBlock.SetColor(_angleColorRef, angleColor);
    _renderer.SetPropertyBlock(_propertyBlock, 0);
    ResetMaterialAngles();
  }

  private IEnumerator RotateAroundCoroutine(Selectable obj) {
    int component = _anchorToComponent[Anchor];
    Vector3 center = obj.transform.position;
    // Stores initial values
    Vector3 initialPointerDirection = GetPointerOnZplane() - center;
    float startAngle = Vector3.SignedAngle(transform.right,
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

      angleRange += angleDelta;
      Debug.Log(angleRange);
      SetMaterialAngleRange(angleRange);
      yield return null;
    }
    ResetMaterialAngles();
  }

  private Vector3 GetPointerOnZplane() {
    Ray pointerRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    Plane zPlane = new Plane(transform.forward, transform.position);
    zPlane.Raycast(pointerRay, out float enter);
    return pointerRay.GetPoint(Mathf.Abs(enter));
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
