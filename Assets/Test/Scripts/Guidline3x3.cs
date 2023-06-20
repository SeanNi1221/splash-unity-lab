using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Guidline3x3 : MonoBehaviour {
  private const float _lineThickness = 0.02f;
  private const float _lineLengthMin = 20;
  private const float _startAngleFade = 15;
  private const float _endAngleFade = 40;
  private const float _centerLineFade = 0.75f;
  private const float _depth = 100;
  public float LineLengthX = _lineLengthMin;
  public float LineLengthY = _lineLengthMin;
  public float width = 1;
  public float height = 1;
  [SerializeField]
  [ColorUsage(true, true)]
  private Color _color = Color.white;
  [SerializeField]
  private DecalProjector[] _projectors = new DecalProjector[0];
  [SerializeField]
  private Material _materialH, _materialHFaded, _materialV, _materialVFaded;

  private void OnValidate() {
    if (_projectors.Length < 6) {
      _projectors = GetComponents<DecalProjector>();
    }
    SetupLines();
  }

  // Better to execute this in edit mode and store the results in serialized fields.
  private void SetupLines() {
    // Common
    foreach (var p in _projectors) {
      p.startAngleFade = _startAngleFade;
      p.endAngleFade = _endAngleFade;
    }

    // Materials
    if (_materialH) {
      _projectors[0].material = _projectors[2].material = _materialH;
    }
    if (_materialHFaded) {
      _projectors[1].material = _materialHFaded;
    }
    if (_materialV) {
      _projectors[3].material = _projectors[5].material = _materialV;
    }
    if (_materialVFaded) {
      _projectors[4].material = _materialVFaded;
    }

    // Colors
    _projectors[0].material.color = _projectors[2].material.color =
        _projectors[3].material.color = _projectors[5].material.color =
            _color;
    _projectors[1].material.color = _projectors[4].material.color =
        new Color(_color.r, _color.g, _color.b, _color.a * (1-_centerLineFade));

    // Sizes
    _projectors[0].size = _projectors[1].size = _projectors[2].size =
        new Vector3(Mathf.Max(LineLengthX, _lineLengthMin), _lineThickness, _depth);
    _projectors[3].size = _projectors[4].size = _projectors[5].size =
        new Vector3(_lineThickness, Mathf.Max(LineLengthY, _lineLengthMin), _depth);
  }

  // TODO: Sizes never get updated after setup. Length members need to be re-arranged.
  public void UpdateLines() {
    // Pivots
    float extentX = width/2;
    float extentY = height/2;
    float extentZ = _depth/2;
    _projectors[0].pivot = new Vector3(0, -extentY, extentZ);
    _projectors[1].pivot = new Vector3(0, 0, extentZ);
    _projectors[2].pivot = new Vector3(0, extentY, extentZ);
    _projectors[3].pivot = new Vector3(-extentX, 0, extentZ);
    _projectors[4].pivot = new Vector3(0, 0, extentZ);
    _projectors[5].pivot = new Vector3(extentX, 0, extentZ);
  }
}
