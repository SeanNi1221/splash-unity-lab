using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// public abstract class FlatState<T> : FlatState {
//   public T Context { get; private set; }
//   protected FlatFSM<T> _fsm;
//   public void PostInitialize(FlatFSM<T> fsm) {
//     Context = fsm.Context;
//   }
// }

public abstract class FlatState {
  protected FlatFSM _fsm;
  public virtual void Enter() { }
  public virtual void Exit() { }
  public virtual void Update() { }

  public virtual void Initialize(FlatFSM fsm) {
    _fsm = fsm;
  }
}

// public abstract class FlatFSM<T> : FlatFSM {
//   public readonly T Context;
//   public FlatFSM(T context) : base() {
//     Context = context;
//     foreach (var state in _availableStates.Values) {
//       ((FlatState<T>)state).PostInitialize(this);
//     }
//   }
// }

public abstract class FlatFSM {
  public System.Enum Current { get; private set; }
  protected Dictionary<System.Enum, FlatState> _availableStates;
  private FlatState _current;

  // The first state in the dictionary is the initial state.
  public FlatFSM() {
    Initialize();
    var enumerator = _availableStates.Keys.GetEnumerator();
    enumerator.MoveNext();
    System.Enum initialState = enumerator.Current;
    Current = initialState;
    _current = _availableStates[initialState];
    _current.Enter();
  }

  public virtual void Update() {
    _current.Update();
  }

  protected virtual void Initialize() {
    foreach (var state in _availableStates.Values) {
      state.Initialize(this);
    }
  }

  protected void SwitchTo(System.Enum state) {
    _current.Exit();
    Current = state;
    _current = _availableStates[state];
    _current.Enter();
  }
}
