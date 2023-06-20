// Copyright 2023 The SeedV Lab (Beijing SeedV Technology Co., Ltd.)
// All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal abstract class CameraControllerBase {
  protected readonly Camera _camera;
  // Has the camera been touched at the current frame?
  protected bool _isDirty;

  public CameraControllerBase(Camera camera) {
    _camera = camera;
  }

  // Call this method inside a MonoBehaviour's LateUpdate to support input
  // listening (E.g. mouse scroll and drag). Use the _isDirty flag to make
  // sure the camera is updated only when needed.
  protected abstract void UpdateCamera();
}
