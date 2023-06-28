using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetButton : Handle , IPointerClickHandler {
  private Vector3 _initialPosition;
  private Quaternion _initialRotation;

  public virtual void OnPointerClick(PointerEventData eventData) {
    GameManager.Selected.transform.position = _initialPosition;
    GameManager.Selected.transform.rotation = _initialRotation;
    Hide();
  }

  public override void SetValue(object value) {
    _initialPosition = ((Transform)value).position;
    _initialRotation = ((Transform)value).rotation;
  }

  protected override void Awake() {
    base.Awake();
    Hide();
  }

  protected virtual void Update() {
    if (Input.GetMouseButtonDown(0) && Hovered != this) {
      Hide();
    }
  }
}
