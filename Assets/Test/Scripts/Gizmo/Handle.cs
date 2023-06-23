using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Handle : Gizmo,
IPointerEnterHandler, IPointerExitHandler,
IPointerDownHandler, IPointerUpHandler {
  [SerializeField] protected Texture2D _horverdCursor;
  [SerializeField] protected Texture2D _draggedCursor;
  [SerializeField] protected Vector2 _cursorHotspot;
  public virtual void OnPointerEnter(PointerEventData eventData) {
    Hovered = this;
    if (_horverdCursor) {
      Cursor.SetCursor(_horverdCursor, _cursorHotspot, CursorMode.Auto);
    }
  }
  public virtual void OnPointerExit(PointerEventData eventData) {
    Hovered = null;
    if (_horverdCursor) {
      Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
}
