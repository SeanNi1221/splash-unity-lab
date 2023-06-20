using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSpaceDrag {
  public readonly Camera Camera;
  private static Ray _pointerRay => TestGameManager.PointerRay;
  private Selectable _obj;
  private Vector3 _offset;
  private Plane _draggingPlane;
  public ViewSpaceDrag(Camera camera, Selectable obj) {
    Camera = camera;
    _obj = obj;
  }

  public void SetPlane() {
    _draggingPlane = new Plane(Camera.transform.forward, _obj.transform.position);
    Vector3 hitPoint = _draggingPlane.Raycast(_pointerRay, out float distance) ?
        _pointerRay.GetPoint(distance) : Vector3.zero;
    _offset = _obj.transform.position - hitPoint;
  }

  public void UpdatePosition() {
    if (_draggingPlane.Raycast(_pointerRay, out float distance)) {
      _obj.transform.position = _pointerRay.GetPoint(distance) + _offset;
    }
  }
}
