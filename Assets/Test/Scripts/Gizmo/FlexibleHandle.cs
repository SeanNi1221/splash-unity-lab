using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FlexibleHandle : Handle, IPointerClickHandler {
  private static readonly Color _normalColor = Color.white;
  private static readonly Color _hoveredColor = new Color(0.8f, 0.8f, 0.8f, 1);
  public static readonly Dictionary<GizmoGroup, IReadOnlyList<GizmoAnchor>> PreferredAnchors =
      new Dictionary<GizmoGroup, IReadOnlyList<GizmoAnchor>>();
  [SerializeField] protected RectTransform _canvasRect;
  [SerializeField] protected GameObject _idleGO;
  [SerializeField] protected GameObject _draggedGO;
  [SerializeField] protected GameObject _selectedGO;
  private Image _idleImage;
  private Image _selectedImage;
  private RectTransform _rectTransform;
  private Vector3 _mouseDownPosition;
  public virtual void OnPointerClick(PointerEventData eventData) {
    if (Input.mousePosition == _mouseDownPosition) {
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
    _mouseDownPosition = Input.mousePosition;
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
    _rectTransform = transform as RectTransform;
    Hide();
  }
  protected override void OnValidate() {
    base.OnValidate();
    if (!_canvasRect) {
      _canvasRect = GetComponentInParent<Canvas>()
          .GetComponent<RectTransform>();
    }
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

  public static IEnumerator AdaptVisibilitiesCoroutine() {
    // Wait after the rect transform is updated to let the in-screen check be
    // available.
    //
    // For some unknown reason 'yied return null' does not work here, and we
    // didn't found a force update solution.
    yield return new WaitForSeconds(0.1f);
    foreach (var pair in PreferredAnchors) {
      var group = pair.Key;
      var anchors = pair.Value;
      for(int i = 0; i < anchors.Count; i++) {
        var flexibleHandle = Catalog[group][anchors[i]] as FlexibleHandle;
        if (flexibleHandle.IsInScreen()) {
          if (!flexibleHandle.isShown) {
            flexibleHandle.ShowUnique();
          }
          break;
        }
      }
    }
    yield return null;
  }

  private bool IsInScreen() {
    return RectTransformUtility.RectangleContainsScreenPoint(
        _canvasRect, _rectTransform.position);
  }
}
