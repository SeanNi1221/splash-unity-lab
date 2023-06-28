using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateIndicator : Indicator
{
  public override void SetValue(object angle) {
    _text.text = $"{((float)angle).ToString("F0")}°";
  }
}
