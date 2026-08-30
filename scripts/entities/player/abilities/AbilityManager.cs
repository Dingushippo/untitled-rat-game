using Godot;

[GlobalClass]
public partial class AbilityManager : Node
{
    [Export] public PlayerAbilityState AbilityState;
    [Export] public MovementAbility[] MovementAbilities;
    [Export] public MovementAbility CurrentMovement;

    [Export] private Player _player;
    [Export] private HierarchicalStateMachine _hfsm;

    public override void _Ready()
    {
        CurrentMovement.Init(_player, _hfsm);
        CurrentMovement.OnEquip();
        AbilityState.ActiveAbility = CurrentMovement;
    }

    public override void _PhysicsProcess(double delta)
    {
        // do while ability is previewing
        CurrentMovement.WhileEquipped();
    }

    public void EquipAbility(MovementAbility ability)
    {
        CurrentMovement.OnUnequip();
        CurrentMovement = ability;

        CurrentMovement.Init(_player, _hfsm);
        CurrentMovement.OnEquip();
        AbilityState.ActiveAbility = CurrentMovement;

    }

    public void ActivateAbility()
    {
        _hfsm.ChangeState<PlayerAbilityState>();
    }

}

public enum AbilityType { Movement, Damage };