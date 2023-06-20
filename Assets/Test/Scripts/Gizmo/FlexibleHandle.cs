using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FlexibleHandle : Handle, IPointerClickHandler {
  private static readonly Color _normalColor = Color.white;
  private static readonly Color _hoveredColor = new Color(0.8f, 0.8f, 0.8f, 1);
  [SerializeField] protected GizmoAnchor _preferredAnchor;
  [SerializeField] protected GameObject _idleGO;
  [SerializeField] protected GameObject _draggedGO;
  [SerializeField] protected GameObject _selectedGO;
  private Image _idleImage;
  private Image _selectedImage;
  public virtual void OnPointerClick(PointerEventData eventData) {
    if (Hovered == this) {
      bool alreadySelected = Selected == this;
      Selected = alreadySelected ? null : this;
      _idleGO.SetActive(alreadySelected);
      _draggedGO.SetActive(false);
      _selectedGO.SetActive(!alreadySelected);
    }
  }

  public override void OnPointerEnter(PointerEventData eventData) {
    base.OnPointerEnter(eventData);
    _idleImage.color = _selectedImage.color = _hoveredColor;
  }

  public override void OnPointerExit(PointerEventData eventData) {
    base.OnPointerExit(eventData);
    _idleImage.color = _selectedImage.color = _normalColor;
  }

  public override void OnPointerDown(PointerEventData eventData) {
    base.OnPointerDown(eventData);
    _idleGO.SetActive(false);
    _draggedGO.SetActive(true);
    _selectedGO.SetActive(false);
  }

  public override void OnPointerUp(PointerEventData eventData) {
    base.OnPointerUp(eventData);
    _idleGO.SetActive(true);
    _draggedGO.SetActive(false);
    _selectedGO.SetActive(false);
  }

  protected virtual void OnDisable() {
    if (Hovered == this) {
      Hovered = null;
    }
    if (Dragged == this) {
      Dragged = null;
    }
    if (Selected == this) {
      Selected = null;
    }
    _idleImage.color = _selectedImage.color = _normalColor;
    _idleGO.SetActive(true);
    _draggedGO.SetActive(false);
    _selectedGO.SetActive(false);
  }

  protected override void Awake() {
    base.Awake();
    _idleImage = _idleGO.GetComponent<Image>();
    _selectedImage = _selectedGO.GetComponent<Image>();
  }
  protected override void OnValidate() {
    base.OnValidate();
    if (!_idleGO) {
      _idleGO = transform.Find("Idle").gameObject;
    }
    if (!_draggedGO) {
      _draggedGO = transform.Find("Dragged").gameObject;
    }
    if (!_selectedGO) {
      _selectedGO = transform.Find("Selected").gameObject;
    }
  }
}
