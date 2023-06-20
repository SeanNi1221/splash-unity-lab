using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectionGizmoRig : MonoBehaviour {
  public static readonly Vector2 ScaleCursorHotspot = new Vector2(8, 8);
  private static readonly Vector2 _paddingInPixel = new Vector2(16, 16);
  public ScaleFsm HandleFsm => _handleFsm;
  public TestGameManager Manager => _manager;
  [SerializeField] private RectTransform _frameRect;
  [SerializeField] private TestGameManager _manager;
  [SerializeField] private Camera _camera;
  private Selectable _selected => _manager.Selected;
  private ScaleFsm _handleFsm;
  private float _inputDeltaX => Input.GetAxis("Mouse X");
  private float _inputDeltaY => Input.GetAxis("Mouse Y");
  private void OnEnable() {
    _frameRect.gameObject.SetActive(true);
    _handleFsm = new ScaleFsm(this);
  }

  private void OnDisable() {
    if (_frameRect) {
      _frameRect.gameObject.SetActive(false);
    }
  }

  private void OnValidate () {
    if (!_frameRect) {
      _frameRect = GameObject.Find("Canvas/SelectionFrame").transform as RectTransform;
    }
    if (!_manager) {
      _manager = FindObjectOfType<TestGameManager>();
    }
    if (!_camera) {
      _camera = Camera.main;
    }
  }

  private void Update() {
    if (_selected) {
      UpdateFrame(_frameRect, _selected.Renderer);
    }
    _handleFsm.Update();
  }

  private void UpdateFrame(RectTransform frame, Renderer renderer) {
    Vector2[] corners = WorldToScreenPoints(GetBoundsCorners(renderer));
    Vector2 min = corners[0];
    Vector2 max = corners[0];
    for (int i = 1; i < corners.Length; i++) {
      min = Vector2.Min(min, corners[i]);
      max = Vector2.Max(max, corners[i]);
    }
    min -= _paddingInPixel;
    max += _paddingInPixel;
    Vector2 size = max - min;
    Vector2 position = min + size / 2;
    frame.sizeDelta = size;
    frame.position = position;
  }

  private Vector2[] WorldToScreenPoints(params Vector3[] points) {
    Vector2[] screenPoints = new Vector2[points.Length];
    for (int i = 0; i < points.Length; i++) {
      screenPoints[i] = _camera.WorldToScreenPoint(points[i]);
    }
    return screenPoints;
  }

  // Get local bounds corners in world space
  private Vector3[] GetBoundsCorners(Renderer r) {
    Bounds b = r.localBounds;
    Vector3[] corners = new Vector3[8];
    corners[0] = b.min;
    corners[1] = new Vector3(b.min.x, b.min.y, b.max.z);
    corners[2] = new Vector3(b.min.x, b.max.y, b.min.z);
    corners[3] = new Vector3(b.min.x, b.max.y, b.max.z);
    corners[4] = new Vector3(b.max.x, b.min.y, b.min.z);
    corners[5] = new Vector3(b.max.x, b.min.y, b.max.z);
    corners[6] = new Vector3(b.max.x, b.max.y, b.min.z);
    corners[7] = b.max;
    for (int i = 0; i < 8; i++) {
      corners[i] = r.transform.TransformPoint(corners[i]);
    }
    return corners;
  }

  private void OnDrawGizmos() {
    if (_selected) {
      var r = _selected.GetComponent<Renderer>();
      if (r) {
        var corners = GetBoundsCorners(r);
        for (int i = 0; i < 8; i++) {
          Gizmos.DrawSphere(corners[i], 0.1f);
        }
      }
    }
  }
}
