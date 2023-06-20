// Copyright 2023 The SeedV Lab (Beijing SeedV Technology Co., Ltd.)
// All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class OrbitController : CameraControllerBase {
  public readonly struct Pose {
    public readonly Vector3 LookAtPos;
    public readonly float Radius;
    public readonly float AngleX;
    public readonly float AngleY;
    public Pose(float angleX, float angleY, float radius, Vector3 lookAtPos) {
      AngleX = angleX;
      AngleY = angleY;
      Radius = radius;
      LookAtPos = lookAtPos;
    }
    public Pose(Camera camera) {
      AngleX = camera.transform.eulerAngles.x;
      AngleY = camera.transform.eulerAngles.y;
      LookAtPos = Vector3.zero;
      Radius = Vector3.Distance(camera.transform.position, LookAtPos);
      camera.transform.LookAt(LookAtPos, Vector3.up);
    }
  }

  // TODO: Add damping for user input.

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
  private const float _rateV = 3000;
  private const float _rateH = 3000;
#elif !UNITY_EDITOR && UNITY_WEBGL
  private const float _rateV = 150;
  private const float _rateH = 150;
#else
  private const float _rateV = 300;
  private const float _rateH = 300;
#endif
  // Constraints.
  private const float _minX = 0;
  private const float _maxX = 90;

  private const float _transitionSeconds = 0.8f;
  private float _angleDeltaY;
  private float _angleDeltaX;
  public readonly Pose InitialPose;
  public Pose CurrentPose { get; private set; }
  public OrbitController(Camera camera) : base(camera) {
    InitialPose = CurrentPose = new Pose(_camera);
  }

  // Smoothly orbits the camera from the current pose to a target pose.
  // Can be used to switch projection.
  public IEnumerator OrbitToCoroutine(Pose targetPose) {
    float startTime = Time.time;
    float endTime = startTime + _transitionSeconds;
    Vector3 startLookAtPos = CurrentPose.LookAtPos;
    float startRadius = CurrentPose.Radius;
    float startAngleX = CurrentPose.AngleX;
    float startAngleY = CurrentPose.AngleY;

    // Alyways choose the nearest angle to rotate.
    float targetAngleX = startAngleX + Mathf.DeltaAngle(startAngleX, targetPose.AngleX);
    float targetAngleY = startAngleY + Mathf.DeltaAngle(startAngleY, targetPose.AngleY);

    while (Time.time <= endTime) {
      float t = (Time.time - startTime) / _transitionSeconds;
      var lookAtPos = new Vector3(
          Mathf.SmoothStep(startLookAtPos.x, targetPose.LookAtPos.x, t),
          Mathf.SmoothStep(startLookAtPos.y, targetPose.LookAtPos.y, t),
          Mathf.SmoothStep(startLookAtPos.z, targetPose.LookAtPos.z, t));

      float radius = Mathf.SmoothStep(startRadius, targetPose.Radius, t);
      float xAngle = Mathf.SmoothStep(startAngleX, targetAngleX, t);
      float yAngle = Mathf.SmoothStep(startAngleY, targetAngleY, t);
      OrbitToImmediately(new Pose(xAngle, yAngle, radius, lookAtPos));
      yield return null;
    }
  }

  // Orbits to the target pose immediately, allows changing radius and look-at position.
  public void OrbitToImmediately(Pose targetPose) {
    Vector3 lookAtDelta = targetPose.LookAtPos - CurrentPose.LookAtPos;
    _camera.transform.Translate(lookAtDelta, Space.World);
    _camera.transform.LookAt(targetPose.LookAtPos, Vector3.up);

    float radiusDelta = targetPose.Radius -
        Vector3.Distance(_camera.transform.position, targetPose.LookAtPos);
    _camera.transform.Translate(0, 0, -radiusDelta, Space.Self);

    OrbitToImmediately(targetPose.AngleX, targetPose.AngleY);
    CurrentPose = targetPose;
  }

  // Orbits to the target angle immediately keeping radius and look-at position.
  public void OrbitToImmediately(float angleX, float angleY) {
    _angleDeltaX = Mathf.DeltaAngle(CurrentPose.AngleX, angleX);
    _angleDeltaY = Mathf.DeltaAngle(CurrentPose.AngleY, angleY);
    UpdateCamera();
  }

  // Orbits by angle delta keeping radius and look-at position.
  public void OrbitBy(float deltaX, float deltaY) {
    _angleDeltaX = -_rateV * deltaY * Time.deltaTime;
    _angleDeltaY = _rateH * deltaX * Time.deltaTime;
    _isDirty = true;
    UpdateCamera();
  }

  protected override void UpdateCamera() {
    // Converts unsigned angle to signed angle.
    float targetAngleX = Mathf.DeltaAngle(0, _camera.transform.eulerAngles.x + _angleDeltaX);

    // Orbits by delta with constraints if the change is made by user input.
    // Otherwise, orbits without constraints.
    if (targetAngleX >= _minX && targetAngleX < _maxX || !_isDirty) {
      _camera.transform.RotateAround(CurrentPose.LookAtPos,
                                      _camera.transform.right, _angleDeltaX);
    }
    _camera.transform.RotateAround(CurrentPose.LookAtPos,
                                    Vector3.up, _angleDeltaY);

    CurrentPose = new Pose(_camera.transform.eulerAngles.x, _camera.transform.eulerAngles.y,
        CurrentPose.Radius, CurrentPose.LookAtPos);
    _isDirty = false;
  }
}
