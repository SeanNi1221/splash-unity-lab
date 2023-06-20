using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {
  public class FSM {
    public class Idle : StateBase {
      public Idle(FSM fsm) : base(fsm) { }
      public override void OnEnter() {
        _fsm.Obj.Renderer.enabled = false;
      }

      public override void OnExit() {
      }

      public override void OnUpdate() {
      }

    }

    public class Hovered : StateBase {
      public Hovered(FSM fsm) : base(fsm) { }
      public override void OnEnter() {
        _fsm.Obj.Renderer.enabled = true;
        _fsm.Obj.Renderer.material.color = _hoveredColor;
      }

      public override void OnExit() {
      }

      public override void OnUpdate() {
      }
    }

    public class Selected : StateBase {
      public Selected(FSM fsm) : base(fsm) { }

      public override void OnEnter() {
        _fsm.Obj.Renderer.material.color = _selectedColor;
      }

      public override void OnExit() {
      }

      public override void OnUpdate() {
      }
    }

    public class Dragging : StateBase {
      public Dragging(FSM fsm) : base(fsm) { }

      public override void OnEnter() {
        _fsm.Obj.Drag.SetPlane();

        _fsm.Manager.GuidlineRig.UpdateGrids(_fsm.Obj.Renderer.bounds);
        _fsm.Manager.GuidlineRig.gameObject.SetActive(true);
      }

      public override void OnExit() {
        _fsm.Manager.GuidlineRig.gameObject.SetActive(false);
      }

      public override void OnUpdate() {
        _fsm.Obj.Drag.UpdatePosition();

        _fsm.Manager.GuidlineRig.UpdatePositions(_fsm.Obj.Renderer.bounds);
      }
    }

    public readonly Selectable Obj;
    public readonly TestGameManager Manager;
    public StateBase Current { get; set; }

    public FSM(Selectable obj, TestGameManager manager) {
      this.Obj = obj;
      this.Manager = manager;
    }
    public void Update() {
      switch (Current) {
        case Idle idle:
          if (Manager.Hovered == Obj) {
            TransitTo(new Hovered(this));
          }
          break;
        case Hovered hovered:
          if (Manager.Hovered == Obj) {
            if (Manager.Selected == Obj) {
              TransitTo(new Selected(this));
            }
          } else TransitTo(new Idle(this));
          break;
        case Selected selected:
          if (Manager.Selected != Obj) {
            TransitTo(new Hovered(this));
          } else if (Manager.IsDragging && !Handle.Dragged) {
            TransitTo(new Dragging(this));
          }
          break;
        case Dragging dragging:
          if (!Manager.IsDragging) {
            TransitTo(new Selected(this));
          }
          break;
        default:
          break;
      }
      Current.OnUpdate();
    }
    private void TransitTo(StateBase state) {
      Current.OnExit();
      Current = state;
      Current.OnEnter();
    }
  }

  public abstract class StateBase {
    protected readonly FSM _fsm;
    protected static Color _selectedColor = new Color(1, 1, 1, 0);
    protected static Color _hoveredColor = new Color(1, 1, 1, 0.05f);
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
    public StateBase(FSM fsm) {
      _fsm = fsm;
    }
  }

  public MeshRenderer Renderer => _renderer;
  public FSM StateMachine { get; private set; }
  public ViewSpaceDrag Drag { get; private set; }
  [SerializeField] private MeshRenderer _renderer;
  private TestGameManager _manager;

  private void OnEnable() {
    _manager = FindObjectOfType<TestGameManager>();
    StateMachine = new FSM(this, _manager);
    StateMachine.Current = new FSM.Idle(StateMachine);
    Drag = new ViewSpaceDrag(Camera.main, this);
  }

  private void OnValidate() {
    if (_renderer == null) {
      _renderer = GetComponent<MeshRenderer>();
    }
  }

  private void Update() {
    StateMachine.Update();
  }
}
