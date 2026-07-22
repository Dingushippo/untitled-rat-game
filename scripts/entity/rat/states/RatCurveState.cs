using System.Numerics;

public class NewRatState : RatState
{
    private Vector3[] _pathArray;
    public NewRatState(Rat owner, Vector3[] pathArray) : base(owner) { _pathArray = pathArray; }
    public override void PhysicsProcess(float delta)
    {

    }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}