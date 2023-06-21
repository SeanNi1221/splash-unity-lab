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

  public SelectableState Current {
    get => _current_OnlyForCache;
    private set {
      // State exit
      switch (_current_OnlyForCache) {
        case SelectableState.Dragged:
          GuidlineRig.Hide();
          break;
        default:
          break;
      }

      _current_OnlyForCache = value;
      // State enter
      switch (value) {
        case SelectableState.Idle:
          _renderer.enabled = false;
          break;
        case SelectableState.Hovered:
          _renderer.enabled = true;
          _renderer.material.color = _hoveredColor;
          break;
        case SelectableState.Selected:
          _renderer.material.color = _selectedColor;
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
  }
  private SelectableState _current_OnlyForCache;
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

  private void Update() {
    // State update;
    switch (Current) {
      case SelectableState.Idle:
        if (GameManager.Hovered == this) {
          Current = SelectableState.Hovered;
        }
        break;
      case SelectableState.Hovered:
        if (GameManager.Hovered == this) {
          if (GameManager.Selected == this) {
            Current = SelectableState.Selected;
          }
        } else Current = SelectableState.Idle;
        break;
      case SelectableState.Selected:
        if (GameManager.Selected != this) {
          Current = SelectableState.Hovered;
        } else if (GameManager.IsDragging && !Handle.Dragged) {
          Current = SelectableState.Dragged;
        }
        break;
      case SelectableState.Dragged:
        ViewSpaceMover.UpdatePosition();
        GuidlineRig.UpdatePositions(_renderer.bounds);
        if (!GameManager.IsDragging) {
          Current = SelectableState.Selected;
        }
        break;
      default:
        break;
    }
  }
}
