using Godot;
using System.Reflection.Metadata;

public class RatCurveState : RatState
{
    public const float MIN_SPEED = 3f;
    public const float MAX_SPEED = 10f;
    private float _progress = 0;

    private int _currentIndex = 0;
    private Vector3[] _pathArray;
    private float _speed;
    public RatCurveState(Rat owner, Vector3[] pathArray, float speed) : base(owner)
    {
        _pathArray = pathArray;
        _speed = speed;
    }
    public override void PhysicsProcess(float delta)
    {
        if (_currentIndex >= _pathArray.Length)
        {
            fsm.ChangeState("landed");
            return;
        }

        Vector3 startPoint = _rat.GlobalPosition;
        Vector3 targetPoint = _pathArray[_currentIndex];

        _rat.LookAt(targetPoint);

        _progress += _speed * delta;
        _rat.GlobalPosition = startPoint.Lerp(targetPoint, _progress);

        if (_rat.GlobalPosition.DistanceSquaredTo(targetPoint) < 1.0)
        {
            _rat.GlobalPosition = targetPoint;
            _progress = 0;
            _currentIndex++;
        }
    }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}