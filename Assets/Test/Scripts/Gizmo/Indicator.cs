using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Indicator : Gizmo {
  [SerializeField] protected TextMeshProUGUI _text;
  public override void SetValue(object text) {
    _text.text = (string)text;
  }

  protected override void Awake() {
    base.Awake();
    Hide();
  }

  protected override void OnValidate() {
    base.OnValidate();
    if (_text == null) {
      _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
  }

}
