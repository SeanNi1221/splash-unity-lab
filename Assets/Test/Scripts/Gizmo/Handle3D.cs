using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle3D : Gizmo {
  protected static readonly Dictionary<GizmoAnchor, Color> _anchorToColor =
      new Dictionary<GizmoAnchor, Color> {
    { GizmoAnchor.X, new Color(1, 0.1f, 0.2f) },
    { GizmoAnchor.Y, new Color(0.2f, 1, 0.1f) },
    { GizmoAnchor.Z, new Color(0, 0.4f, 1) },
  };

  protected const float _normalAlpha = 0.3f;
  protected const float _hoveredAlpha = 0.5f;
  protected const float _draggedAlpha = 1f;
  private const string _baseColorRef = "_BaseColor";
  [SerializeField] protected Renderer _renderer;
  private MaterialPropertyBlock _propertyBlock;
  private Color _normalColor;
  private Color _hoveredColor;
  private Color _draggedColor;


  protected virtual void OnEnable() {
    transform.position = GameManager.Selected.transform.position;
  }

  protected virtual void OnMouseEnter() {
    if (Dragged != this) {
      Hovered = this;
      _propertyBlock.SetColor(_baseColorRef, _hoveredColor);
      _renderer.SetPropertyBlock(_propertyBlock, 0);
    }
  }

  protected virtual void OnMouseExit() {
    if (Dragged != this && Hovered == this) {
      Hovered = null;
      _propertyBlock.SetColor(_baseColorRef, _normalColor);
      _renderer.SetPropertyBlock(_propertyBlock, 0);
    }
  }

  protected virtual void OnMouseDown() {
    Dragged = this;
    _propertyBlock.SetColor(_baseColorRef, _draggedColor);
    _renderer.SetPropertyBlock(_propertyBlock, 0);
    HideGroup(GizmoGroup.Frame);
    HideGroup(GizmoGroup.MoveHandle);
    HideGroup(GizmoGroup.RotateHandle);
  }

  protected virtual void OnMouseUp() {
    if (Dragged == this) {
      Dragged = null;
      _propertyBlock.SetColor(_baseColorRef, _normalColor);
      _renderer.SetPropertyBlock(_propertyBlock, 0);
      ShowGroup(GizmoGroup.Frame);
      StartCoroutine(FlexibleHandle.AdaptVisibilitiesCoroutine());
    }
  }

  protected override void Awake() {
    base.Awake();
    Hide();
    _normalColor = new Color(_anchorToColor[Anchor].r,
                             _anchorToColor[Anchor].g,
                             _anchorToColor[Anchor].b,
                             _normalAlpha);
    _hoveredColor = new Color(_anchorToColor[Anchor].r,
                              _anchorToColor[Anchor].g,
                              _anchorToColor[Anchor].b,
                              _hoveredAlpha);
    _draggedColor = new Color(_anchorToColor[Anchor].r,
                              _anchorToColor[Anchor].g,
                              _anchorToColor[Anchor].b,
                              _draggedAlpha);
    _propertyBlock = new MaterialPropertyBlock();
    _renderer.GetPropertyBlock(_propertyBlock, 0);
    _propertyBlock.SetColor(_baseColorRef, _normalColor);

    _renderer.SetPropertyBlock(_propertyBlock, 0);
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
