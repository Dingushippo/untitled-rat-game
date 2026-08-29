// using Godot;

// public class PlayerArcMovementState : PlayerState
// {
//     public PlayerArcMovementState(Player owner)
//         : base(owner) { }

//     private Vector3 _targetPosition;
//     private float _arcHeight;
//     private float _moveDuration;
//     private Vector3 _startPosition;

//     public void Configure(Vector3 target, float arcHeight)
//     {
//         _targetPosition = target;
//         _arcHeight = Mathf.Max(arcHeight, _player.Tuning.ArcMinHeight);

//         float distance = _player.GlobalPosition.DistanceTo(target);
//         _moveDuration = distance / _player.Tuning.ArcMoveSpeed;
//     }

//     public override void Enter(State previous = null)
//     {
//         _startPosition = _player.GlobalPosition;
//         _weight = 0f;

//         Tween tween = _player.CreateTween();
//         tween.SetEase(_player.Tuning.ArcEase);
//         tween.SetTrans(_player.Tuning.ArcTrans);
//         tween.TweenMethod(Callable.From<float>(SetWeight), 0f, 1f, _moveDuration);
//     }

//     private void SetWeight(float w) => _weight = w;

//     private float _weight;

//     public override void IntegrateForces(PhysicsDirectBodyState3D state)
//     {
//         // lerp between start and target
//         Vector3 currentLinear = _startPosition.Lerp(_targetPosition, _weight);

//         // add arc offset
//         float arc = _player.Tuning.ArcHeightMultiplier * _arcHeight * _weight * (1.0f - _weight);
//         Vector3 arcOffset = (Vector3.Up * arc).Abs();
//         Vector3 targetPoint = currentLinear + arcOffset;

//         Vector3 distanceToTarget = targetPoint - state.Transform.Origin;

//         Vector3 desiredVelocity = distanceToTarget / (float)state.Step;
//         state.LinearVelocity = desiredVelocity;

//         if (_weight >= 1.0)
//         {
//             Transform3D t = state.Transform;
//             t.Origin = _targetPosition;
//             state.Transform = t;
//             _hfsm.ChangeState<PlayerFallingState>(this);
//         }
//     }
// }
