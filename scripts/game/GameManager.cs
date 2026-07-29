using Godot;

public partial class GameManager : Node
{
    private FiniteStateMachine _fsm;

    public override void _Ready()
    {
        _fsm = new(this);
    }

}