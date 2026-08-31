using Godot;
using System;

[GlobalClass]
public partial class ProjectileComponent : Area3D, ISpellComponent
{
    [Export] public float Speed = 10;
    [Export] public float GravityStrength = 1f;


    public SpellPayload Payload { get; set; }
    public Action<SpellPayload> OnComplete { get; set; }

    private Vector3 _direction;
    private Vector3 _velocity;
    private Node3D _spell;

    public void Initialize(Node3D spell, SpellPayload payload)
    {
        Payload = payload;
        _spell = spell;
        _direction = GlobalPosition.DirectionTo(Payload.TargetPosition);
        _velocity = _direction * Speed;
        LookAt(GlobalPosition + _direction);

        BodyEntered += Completed;
    }

    public void Process(float delta)
    {
        float _gravityForce = _velocity.Y - GravityStrength * delta;
        _velocity = _velocity with { Y = _gravityForce };
        _spell.GlobalPosition += _velocity * delta;

        if (_velocity.LengthSquared() > 0.001)
            _spell.LookAt(_spell.GlobalPosition + _velocity, Vector3.Up);
    }

    public void Completed(Node3D body)
    {
        BodyEntered -= Completed;
        OnComplete?.Invoke(Payload);
    }
}