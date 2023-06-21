using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidlineRig : MonoBehaviour {
  private static GuidlineRig _instance;
  [SerializeField]
  private Guidline3x3 _down, _forward, _left, _right;

  public static void UpdateGrids(Bounds source) {
    _instance.UpdateSizes(source);
    _instance.UpdateLenghts(source);
    _instance._down.UpdateLines();
    _instance._forward.UpdateLines();
    _instance._left.UpdateLines();
    _instance._right.UpdateLines();
  }

  public static void UpdatePositions(Bounds source) {
    _instance._down.transform.position = new Vector3(source.center.x, source.min.y, source.center.z);
    _instance._forward.transform.position = new Vector3(source.center.x, source.center.y, source.max.z);
    _instance._left.transform.position = new Vector3(source.min.x, source.center.y, source.center.z);
    _instance._right.transform.position = new Vector3(source.max.x, source.center.y, source.center.z);
  }

  public static void Show() {
    _instance.gameObject.SetActive(true);
  }

  public static void Hide() {
    _instance.gameObject.SetActive(false);
  }

  public void Awake() {
    _instance = this;
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
