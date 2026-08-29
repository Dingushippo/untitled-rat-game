using Godot;


[GlobalClass]
public abstract partial class MovementAbility : Resource
{
    protected Player _player;
    protected HierarchicalStateMachine _hfsm;

    public virtual void Init(Player player, HierarchicalStateMachine hfsm)
    {
        _player = player;
        _hfsm = hfsm;
    }

    public abstract void OnActivate();

    public abstract void PhysicsProcess(float delta);

    public abstract void OnDeactivate();
}