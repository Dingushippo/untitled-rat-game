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
    public abstract void PhysicsProcess(float delta);
    public virtual void OnActivate() { }
    public virtual void OnDeactivate() { }
    public virtual void OnEquip() { }
    public virtual void OnUnequip() { }
    public virtual void WhileEquipped() { } // for preview visuals and such
}