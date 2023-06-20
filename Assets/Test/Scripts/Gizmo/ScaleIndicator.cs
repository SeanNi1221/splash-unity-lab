using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleIndicator : Indicator
{
  public override void SetValue(object scale) {
    _text.text = ((float)scale).ToString("P0");
  }
}
