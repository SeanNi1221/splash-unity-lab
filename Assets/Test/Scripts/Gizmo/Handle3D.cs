using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle3D : Gizmo {
  [SerializeField] protected Color _normalColor = new Color(0.490196f, 1f, 0.3137253f, 0.3f);
  [SerializeField] protected Color _hoveredColor = new Color(0.490196f, 1f, 0.3137253f, 0.5f);
  [SerializeField] protected Color _draggedColor = new Color(0.490196f, 1f, 0.3137253f, 1f);
  [SerializeField] protected Renderer _renderer;

  protected virtual void OnEnable() {
    transform.position = GameManager.Selected.transform.position;
  }

  protected virtual void OnMouseEnter() {
    if (Dragged != this) {
      Hovered = this;
      _renderer.material.color = _hoveredColor;
    }
  }

  protected virtual void OnMouseExit() {
    if (Dragged != this && Hovered == this) {
      Hovered = null;
      _renderer.material.color = _normalColor;
    }
  }

  protected virtual void OnMouseDown() {
    Dragged = this;
    _renderer.material.color = _draggedColor;
    HideGroup(GizmoGroup.Frame);
    HideGroup(GizmoGroup.MoveHandle);
    HideGroup(GizmoGroup.RotateHandle);
  }

  protected virtual void OnMouseUp() {
    if (Dragged == this) {
      Dragged = null;
      _renderer.material.color = _normalColor;
      ShowGroup(GizmoGroup.Frame);
      StartCoroutine(FlexibleHandle.AdaptVisibilitiesCoroutine());
    }
  }

  protected override void Awake() {
    base.Awake();
    Hide();
    _renderer.material.color = _normalColor;
  }

  protected override void OnValidate() {
    base.OnValidate();
    if (!_renderer) {
      _renderer = GetComponent<MeshRenderer>();
    }
  }

  protected virtual void Update() {
    if (Dragged) {
      transform.position = GameManager.Selected.transform.position;
    }
  }
}
