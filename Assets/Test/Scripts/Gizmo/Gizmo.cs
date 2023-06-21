using System.Collections.Generic;
using UnityEngine;

public enum GizmoGroup {
  Default,
  Frame,
  MoveHandle,
  ScaleIndicator,
  AngleIndicator,
}

public enum GizmoAnchor {
  Default,
  TopLeft,
  TopRight,
  BottomLeft,
  BottomRight,
  Center,
  Top,
  Bottom,
  Left,
  Right,
}

public class Gizmo : MonoBehaviour {
  public static readonly Dictionary<GizmoGroup, Dictionary<GizmoAnchor, Gizmo>> Catalog =
      new Dictionary<GizmoGroup, Dictionary<GizmoAnchor, Gizmo>>();

  public GizmoGroup Group => _group;
  public GizmoAnchor Anchor => _anchor;
  [SerializeField] protected GizmoGroup _group;
  [SerializeField] protected GizmoAnchor _anchor;

#if UNITY_EDITOR
  private static readonly Dictionary<string, GizmoGroup> _prefixToType =
      new Dictionary<string, GizmoGroup>() {
          { "Frame", GizmoGroup.Frame },
          { "MoveHandle", GizmoGroup.MoveHandle },
          { "ScaleIndicator", GizmoGroup.ScaleIndicator },
          { "AngleIndicator", GizmoGroup.AngleIndicator },
      };

  private static readonly Dictionary<string, GizmoAnchor> _suffixToAnchor =
      new Dictionary<string, GizmoAnchor>() {
          { "TL", GizmoAnchor.TopLeft },
          { "TR", GizmoAnchor.TopRight },
          { "BL", GizmoAnchor.BottomLeft },
          { "BR", GizmoAnchor.BottomRight },
          { "Center", GizmoAnchor.Center },
          { "Top", GizmoAnchor.Top },
          { "Bottom", GizmoAnchor.Bottom },
          { "Left", GizmoAnchor.Left },
          { "Right", GizmoAnchor.Right },
      };
#endif

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
    if (_prefixToType.TryGetValue(nameParts[0], out GizmoGroup type)) {
      _group = type;
    }

    if (nameParts.Length > 1 && _suffixToAnchor.TryGetValue(nameParts[1], out GizmoAnchor anchor)) {
      _anchor = anchor;
    }
  }
}
