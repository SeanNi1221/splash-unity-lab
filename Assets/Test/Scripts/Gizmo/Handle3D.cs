using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle3D : Gizmo {
  protected static Color _normalColor = new Color(0.490196f, 1f, 0.3137253f, 0.1f);
  protected static Color _hoveredColor = new Color(0.490196f, 1f, 0.3137253f, 0.5f);
  protected static Color _draggedColor = new Color(0.490196f, 1f, 0.3137253f, 1f);
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
    GameManager.Selected.gameObject.GetComponent<Collider>().enabled = true;
    if (Dragged != this && Hovered == this) {
      Hovered = null;
      _renderer.material.color = _normalColor;
    }
  }

  protected virtual void OnMouseDrag() {
    Dragged = this;
    _renderer.material.color = _draggedColor;
  }

  protected virtual void OnMouseUp() {
    if (Dragged == this) {
      Dragged = null;
      _renderer.material.color = _normalColor;
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
}
