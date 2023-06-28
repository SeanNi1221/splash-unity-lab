using System.Collections.Generic;
using UnityEngine;
using System;

public enum GizmoGroup {
  Undefined,
  Frame,
  MoveHandle,
  MoveHandle3D,
  RotateHandle,
  RotateHandle3D,
  RotateIndicator,
  ScaleIndicator,
  AngleIndicator,
}

public enum GizmoAnchor {
  Undefined,
  Center,
  Top, Bottom, Left, Right,
  TC, TL, TR, BL, BR,
  X, Y, Z,
}

public class Gizmo : MonoBehaviour {
  public static Gizmo Hovered { get; protected set; }
  public static Gizmo Dragged { get; protected set; }
  public static Gizmo Selected { get; protected set; }
  public static readonly Dictionary<GizmoGroup, Dictionary<GizmoAnchor, Gizmo>> Catalog =
      new Dictionary<GizmoGroup, Dictionary<GizmoAnchor, Gizmo>>();

  public GizmoGroup Group => _group;
  public GizmoAnchor Anchor => _anchor;
  [SerializeField] protected GizmoGroup _group;
  [SerializeField] protected GizmoAnchor _anchor;

  public bool isShown => gameObject.activeSelf;

  public static void ShowGroup(GizmoGroup group) {
    if (Catalog.TryGetValue(group, out Dictionary<GizmoAnchor, Gizmo> gizmos)) {
      foreach (Gizmo gizmo in gizmos.Values) {
        gizmo.Show();
      }
    }
  }

  public static void HideGroup(GizmoGroup group) {
    if (Catalog.TryGetValue(group, out Dictionary<GizmoAnchor, Gizmo> gizmos)) {
      foreach (Gizmo gizmo in gizmos.Values) {
        gizmo.Hide();
      }
    }
  }

  public void Show() {
    gameObject.SetActive(true);
  }

  public void Hide() {
    gameObject.SetActive(false);
  }

  public void ShowUnique() {
    if (Catalog.TryGetValue(_group, out Dictionary<GizmoAnchor, Gizmo> gizmos)) {
      foreach (Gizmo gizmo in gizmos.Values) {
        gizmo.Hide();
      }
    }
    Show();
  }

  public virtual object CalcValue() {
    Debug.LogWarning($"CalcValue not implemented for type {this.GetType()}. No operation performed.");
    return null;
  }

  public virtual void SetValue(object value) {
    Debug.LogWarning($"SetValue not implemented for type {this.GetType()}. No operation performed.");
  }

  public virtual void SetValues(params object[] values) {
    Debug.LogWarning($"SetValues not implemented for type {this.GetType()}. No operation performed.");
  }

  protected virtual void Awake() {
    if (!Catalog.TryGetValue(_group, out var _)) {
      Catalog[_group] = new Dictionary<GizmoAnchor, Gizmo>();
    }
    Catalog[_group][_anchor] = this;
  }

  protected virtual void OnValidate() {
    string[] nameParts = name.Split('_');
    if (nameParts.Length > 0 && Enum.TryParse(nameParts[0], out GizmoGroup group)) {
      _group = group;
    }
    if (nameParts.Length > 1 && Enum.TryParse(nameParts[1], out GizmoAnchor anchor)) {
      _anchor = anchor;
    }
  }
}
