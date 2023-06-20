using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidlineRig : MonoBehaviour {
  [SerializeField]
  private Guidline3x3 _down, _forward, _left, _right;

  public void UpdateGrids(Bounds source) {
    UpdateSizes(source);
    UpdateLenghts(source);
    _down.UpdateLines();
    _forward.UpdateLines();
    _left.UpdateLines();
    _right.UpdateLines();
  }

  public void UpdatePositions(Bounds source) {
    _down.transform.position = new Vector3(source.center.x, source.min.y, source.center.z);
    _forward.transform.position = new Vector3(source.center.x, source.center.y, source.max.z);
    _left.transform.position = new Vector3(source.min.x, source.center.y, source.center.z);
    _right.transform.position = new Vector3(source.max.x, source.center.y, source.center.z);
  }

  private void OnValidate() {
    if (!_down) {
      _down = transform.Find("GuideLine3x3_Y-").GetComponent<Guidline3x3>();
    }
    if (!_forward) {
      _forward = transform.Find("GuideLine3x3_Z+").GetComponent<Guidline3x3>();
    }
    if (!_left) {
      _left = transform.Find("GuideLine3x3_X-").GetComponent<Guidline3x3>();
    }
    if (!_right) {
      _right = transform.Find("GuideLine3x3_X+").GetComponent<Guidline3x3>();
    }
  }

  private void UpdateSizes(Bounds source) {
    _down.width = _forward.width = source.size.x;
    _left.width = _right.width = _down.height = source.size.z;
    _left.height = _right.height = _forward.height = source.size.y;
  }

  private void UpdateLenghts(Bounds source) {
    _down.LineLengthX = _forward.LineLengthX = source.size.x * 4;
    _left.LineLengthX = _right.LineLengthX = _down.LineLengthY = source.size.z * 4;
    _left.LineLengthY = _right.LineLengthY = _forward.LineLengthY = source.size.y * 4;
  }
}
