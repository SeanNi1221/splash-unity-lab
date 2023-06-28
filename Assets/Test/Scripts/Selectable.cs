using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectableState {
  Idle,
  Hovered,
  Selected,
  Dragged,
}

public class Selectable : MonoBehaviour {
  private static Color _selectedColor = new Color(1, 1, 1, 0);
  private static Color _hoveredColor = new Color(1, 1, 1, 0.05f);

  public MeshRenderer Renderer => _renderer;
  public ViewSpaceMover ViewSpaceMover { get; private set; }
  [SerializeField] private MeshRenderer _renderer;
  [SerializeField] private GameManager _gameManager;

  public SelectableState Current { get; private set; }

  public void TransitTo(SelectableState state) {
    if (Current == state) {
      Debug.LogWarning($"The state is already {state}");
      return;
    }

    // State exit
    switch (Current) {
      case SelectableState.Selected:
        break;
      case SelectableState.Dragged:
        GuidlineRig.Hide();
        break;
      default:
        break;
    }

    Current = state;
    // State enter
    switch (Current) {
      case SelectableState.Idle:
        _renderer.enabled = false;
        break;
      case SelectableState.Hovered:
        _renderer.enabled = true;
        _renderer.material.color = _hoveredColor;
        break;
      case SelectableState.Selected:
        _renderer.material.color = _selectedColor;
        StartCoroutine(FlexibleHandle.AdaptVisibilitiesCoroutine());
        Gizmo.HideGroup(GizmoGroup.ResetButton);
        break;
      case SelectableState.Dragged:
        ViewSpaceMover.SetPlane();
        GuidlineRig.UpdateGrids(_renderer.bounds);
        GuidlineRig.Show();
        break;
      default:
        break;
    }
  }

  private void OnEnable() {
    ViewSpaceMover = new ViewSpaceMover(Camera.main, this);
  }

  private void OnValidate() {
    if (!_gameManager) {
      _gameManager = FindObjectOfType<GameManager>();
    }
    if (!_renderer) {
      _renderer = GetComponent<MeshRenderer>();
    }
  }

  private void LateUpdate() {
    // State update;
    switch (Current) {
      case SelectableState.Idle:
        if (GameManager.Hovered == this) {
          TransitTo(SelectableState.Hovered);
        }
        break;
      case SelectableState.Hovered:
        if (GameManager.Hovered == this) {
          if (GameManager.Selected == this) {
            TransitTo(SelectableState.Selected);
          }
        } else {
          TransitTo(SelectableState.Idle);
        }
        break;
      case SelectableState.Selected:
        if (GameManager.Selected != this) {
          TransitTo(SelectableState.Hovered);
        } else if (GameManager.IsDragging && !Gizmo.Dragged && !Gizmo.Hovered) {
          TransitTo(SelectableState.Dragged);
        }
        break;
      case SelectableState.Dragged:
        ViewSpaceMover.UpdatePosition();
        GuidlineRig.UpdatePositions(_renderer.bounds);
        if (!GameManager.IsDragging) {
          TransitTo(SelectableState.Selected);
        }
        break;
      default:
        break;
    }
  }
}
