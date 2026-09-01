using Godot;
using System;

[GlobalClass]
public partial class ProjectileComponent : Area3DSpellComponent
{
    [Export] public float Speed = 10;
    [Export] public float GravityStrength = 1f;

    private Vector3 _direction;
    private Vector3 _velocity;

    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        base.Initialize(spell, payload);

        _direction = GlobalPosition.DirectionTo(_payload.TargetPosition);
        _velocity = _direction * Speed;

        LookAt(GlobalPosition + _direction);

        BodyEntered += Completed;
    }

    public override void Process(float delta)
    {
        float _gravityForce = _velocity.Y - GravityStrength * delta;
        _velocity = _velocity with { Y = _gravityForce };
        _spell.GlobalPosition += _velocity * delta;

        if (_velocity.LengthSquared() > 0.001)
            _spell.LookAt(_spell.GlobalPosition + _velocity, Vector3.Up);
    }

    public void Completed(Node3D body)
    {
        _payload.TargetNodes.Add(body);
        BodyEntered -= Completed;
        RaiseComplete(_payload);
    }
}