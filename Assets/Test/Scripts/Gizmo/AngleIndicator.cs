using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleIndicator : Indicator
{
  public override void SetValue(object angle) {
    _text.text = $"{((float)angle).ToString("F0")}°";
  }
}
