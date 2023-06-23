using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
  public static Selectable Hovered { get; private set;}
  public static Selectable Selected { get; private set;}
  public static bool IsDragging { get; private set; }
  public bool IsOrbiting { get; private set; }
  [SerializeField] private SelectionGizmoRig _selectionGizmoRig;
  [SerializeField] private GuidlineRig _guidlineRig;
  private OrbitController _controller;
  private Ray _pointerRay => Camera.main.ScreenPointToRay(Input.mousePosition);

  private void OnValidate() {
    if (!_selectionGizmoRig) {
      _selectionGizmoRig = GetComponent<SelectionGizmoRig>();
    }
    if (!_guidlineRig) {
      _guidlineRig = GetComponentInChildren<GuidlineRig>();
    }
  }

  void Awake() {
    _controller = new OrbitController(Camera.main);
    _guidlineRig.Awake();
    FlexibleHandle.PreferredAnchors[GizmoGroup.MoveHandle] = new List<GizmoAnchor>() {
        GizmoAnchor.Right,
        GizmoAnchor.Left,
    };
  }

  void Update() {
    TryHover();
    TrySelect();
    IsOrbiting = Input.GetMouseButton(1) || Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt);
    IsDragging = Input.GetMouseButton(0) && Selected && !IsOrbiting;
    if (IsOrbiting) {
      _controller.OrbitBy(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    _selectionGizmoRig.enabled = Selected && Selected.Current != SelectableState.Dragged
        && !IsOrbiting;
  }

  private void TryHover() {
    if (Handle.Hovered) {
      Hovered = null;
      return;
    }
    if (Physics.Raycast(_pointerRay, out RaycastHit hit, 1000, 1 << LayerMask.NameToLayer("Selectable"))) {
      var selectable = hit.collider.GetComponent<Selectable>();
      if (selectable) {
        Hovered = selectable;
        return;
      }
    }
    Hovered = null;
  }

  private void TrySelect() {
    if (Input.GetMouseButtonDown(0)) {
      if (Gizmo.Hovered) {
        return;
      } else if (Hovered) {
        Selected = Hovered;
      } else {
        Selected = null;
      }
    }
  }
}

