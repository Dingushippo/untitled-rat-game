using Godot;
using System.Reflection.Metadata;

public class RatCurveState : RatState
{
    public const float MIN_SPEED = 3f;
    public const float MAX_SPEED = 10f;
    public WorkSlot WorkSlot = null;
    private float _progress = 0;
    private int _currentIndex = 0;
    private Vector3[] _pathArray;
    private float _speed;
    public RatCurveState(Rat owner, Vector3[] pathArray, float speed, WorkSlot slot = null) : base(owner)
    {
        _pathArray = pathArray;
        _speed = speed;
        WorkSlot = slot;
    }
    public override void PhysicsProcess(float delta)
    {
        if (_currentIndex >= _pathArray.Length)
        {
            string nextState = WorkSlot == null ? "landed" : "slotted";
            fsm.ChangeState(nextState, this);
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
}