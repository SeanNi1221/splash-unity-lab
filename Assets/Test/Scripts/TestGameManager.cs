using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour
{
  public Selectable Hovered { get; private set;}
  public  Selectable Selected { get; private set;}
  public bool IsDragging { get; private set; }
  public bool IsOrbiting { get; private set; }
  public SelectionGizmoRig SelectionGizmoRig => _selectionGizmoRig;
  public GuidlineRig GuidlineRig => _guidlineRig;
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
  }

  void Update() {
    TryHover();
    TrySelect();
    IsOrbiting = Input.GetMouseButton(1) || Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt);
    IsDragging = Input.GetMouseButton(0) && Selected && !IsOrbiting;
    if (IsOrbiting) {
      _controller.OrbitBy(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    SelectionGizmoRig.enabled = Selected && !IsOrbiting && !IsDragging || Handle.Dragged;
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
      if (Handle.Hovered) {
        return;
      } else if (Hovered) {
        Selected = Hovered;
      } else {
        Selected = null;
      }
    }
  }
}

