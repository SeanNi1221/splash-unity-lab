using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFsm : FlatFSM<SelectionGizmoRig> {
  public enum State {
    Idle,
    Hovered,
    Dragging,
  }

  private class Idle : FlatState<SelectionGizmoRig> {
    public override void Enter() {
    }
  }

  private class Hovered : FlatState<SelectionGizmoRig> {
  }

  private class Dragging : FlatState<SelectionGizmoRig> {
    private Gizmo visibleIndicator;
    public override void Enter() {
      visibleIndicator =
          Gizmo.Catalog[GizmoGroup.ScaleIndicator][Handle.Dragged.Anchor];
      visibleIndicator.Show();
    }

    public override void Update() {
      var handle = Handle.Dragged;
      float scaleDelta = (float)handle.CalcValue();
      Transform selectedObject = Context.Manager.Selected.transform;
      selectedObject.localScale *= 1 + scaleDelta;
      visibleIndicator.SetValue(Mathf.Max(selectedObject.localScale.x,
                                          selectedObject.localScale.y,
                                          selectedObject.localScale.z
      ));
    }

    public override void Exit() {
      visibleIndicator.Hide();
    }
  }

  public ScaleFsm(SelectionGizmoRig context) : base(context) { }

  public override void Update() {
    switch (Current) {
      case State.Idle:
        if (Handle.Hovered is ScaleHandle) {
          SwitchTo(State.Hovered);
        }
        break;
      case State.Hovered:
        if (Input.GetMouseButton(0)) {
          SwitchTo(State.Dragging);
        } else if (!Handle.Hovered) {
          SwitchTo(State.Idle);
        }
        break;
      case State.Dragging:
        if (!Input.GetMouseButton(0)) {
          SwitchTo(State.Idle);
        }
        break;
      default:
        break;
    }
    base.Update();
  }
  protected override void Initialize() {
    _availableStates = new Dictionary<System.Enum, FlatState> {
      { State.Idle, new Idle() },
      { State.Hovered, new Hovered() },
      { State.Dragging, new Dragging() },
    };
    base.Initialize();
  }
}
