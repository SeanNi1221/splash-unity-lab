using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleHandle : Handle {
  private const float _scaleRatio = 0.1f;
  private static Vector2 _forceCursorHotspot = new Vector2(8, 8);
  private static float _inputDeltaX => Input.GetAxis("Mouse X");
  private static float _inputDeltaY => Input.GetAxis("Mouse Y");
  private static Dictionary<GizmoAnchor, System.Func<float>> _anchorToScaleDelta =
      new Dictionary<GizmoAnchor, System.Func<float>>() {
        [GizmoAnchor.TopLeft] = () => {
          float rawDelta = _inputDeltaX < _inputDeltaY ?
              Mathf.Max(-_inputDeltaX, _inputDeltaY) : Mathf.Min(-_inputDeltaX, _inputDeltaY);
          return rawDelta * _scaleRatio;
        },
        [GizmoAnchor.TopRight] = () => {
          float rawDelta = _inputDeltaX > -_inputDeltaY ?
              Mathf.Max(_inputDeltaX, _inputDeltaY) : Mathf.Min(_inputDeltaX, _inputDeltaY);
          return rawDelta * _scaleRatio;
        },
        [GizmoAnchor.BottomLeft] = () => {
          float rawDelta = _inputDeltaX < -_inputDeltaY ?
              Mathf.Max(-_inputDeltaX, -_inputDeltaY) : Mathf.Min(-_inputDeltaX, -_inputDeltaY);
          return rawDelta * _scaleRatio;
        },
        [GizmoAnchor.BottomRight] = () => {
          float rawDelta = _inputDeltaX > _inputDeltaY ?
              Mathf.Max(_inputDeltaX, -_inputDeltaY) : Mathf.Min(_inputDeltaX, -_inputDeltaY);
          return rawDelta * _scaleRatio;
        },
      };
# if UNITY_EDITOR
  private static Dictionary<GizmoAnchor, string> _anchorToCursor =
      new Dictionary<GizmoAnchor, string>() {
        [GizmoAnchor.TopLeft] = "Assets/Test/Sprites/ScaleCursorTL.png",
        [GizmoAnchor.TopRight] = "Assets/Test/Sprites/ScaleCursorTR.png",
        [GizmoAnchor.BottomLeft] = "Assets/Test/Sprites/ScaleCursorTR.png",
        [GizmoAnchor.BottomRight] = "Assets/Test/Sprites/ScaleCursorTL.png",
      };
# endif

  public override void OnPointerDown(PointerEventData eventData) {
    base.OnPointerDown(eventData);
    if (Dragged == this) {
      StartCoroutine(ScaleCoroutine());
    }
  }
  protected override void OnValidate() {
    base.OnValidate();
    _cursorHotspot = _forceCursorHotspot;
# if UNITY_EDITOR
    if ((!_horverdCursor || !_draggedCursor) && _anchorToCursor.TryGetValue(Anchor, out string cursorPath)) {
      var texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(cursorPath);
      if (texture) {
        _horverdCursor = _draggedCursor = texture;
      }
    }
#endif
  }

  private IEnumerator ScaleCoroutine() {
    Gizmo indicator = Gizmo.Catalog[GizmoGroup.ScaleIndicator][Anchor];
    indicator.Show();
    while (Dragged == this) {
      float scaleDelta = _anchorToScaleDelta[Anchor]();
      Transform t = _gameManager.Selected.transform;
      t.localScale *= 1 + scaleDelta;
      indicator.SetValue(Mathf.Max(t.localScale.x, t.localScale.y, t.localScale.z));
      yield return null;
    }
    indicator.Hide();
  }
}
