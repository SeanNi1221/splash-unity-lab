using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Handle : Gizmo,
IPointerEnterHandler, IPointerExitHandler,
IPointerDownHandler, IPointerUpHandler {
  protected static readonly Color _normalColor = Color.white;
  protected static readonly Color _hoveredColor = new Color(0.8f, 0.8f, 0.8f, 1);
  [SerializeField] protected Texture2D _horverdCursor;
  [SerializeField] protected Texture2D _draggedCursor;
  [SerializeField] protected Vector2 _cursorHotspot;
  [SerializeField] protected Image _image;
  public virtual void OnPointerEnter(PointerEventData eventData) {
    Hovered = this;
    if (_horverdCursor) {
      Cursor.SetCursor(_horverdCursor, _cursorHotspot, CursorMode.Auto);
    }
    if (_image) {
      _image.color = _hoveredColor;
    }
  }
  public virtual void OnPointerExit(PointerEventData eventData) {
    Hovered = null;
    if (_horverdCursor) {
      Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    if (_image) {
      _image.color = _normalColor;
    }
  }
  public virtual void OnPointerDown(PointerEventData eventData) {
    Dragged = this;
    if (_draggedCursor) {
      Cursor.SetCursor(_draggedCursor, _cursorHotspot, CursorMode.Auto);
    }
  }
  public virtual void OnPointerUp(PointerEventData eventData) {
    Dragged = null;
    if (_draggedCursor) {
      Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
  }
  protected override void OnValidate() {
    base.OnValidate();
    if (!_image) {
      _image = GetComponent<Image>();
    }
  }
}
