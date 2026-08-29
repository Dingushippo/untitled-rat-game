using Godot;

public abstract partial class State : Node
{
    public State Parent;
    protected Node _owner;

    protected HierarchicalStateMachine _hfsm;

    public virtual void Enter(State previous = null) { }

    public virtual void Exit() { }

    public virtual void PhysicsProcess(float delta) { }

    public virtual void Process(float delta) { }

    public virtual void Init(Node owner, HierarchicalStateMachine stateMachine, State parent = null)
    {
        _owner = owner;
        _hfsm = stateMachine;
        Parent = parent;
    }
}
