using Godot;

[GlobalClass]
public partial class ProjectileComponent : Area3DSpellComponent
{
    [Export] public float Speed = 10;
    [Export] public float GravityStrength = 1f;

    [Export] public float AdditionalTravelAfterCollision = 0f;

    private Vector3 _direction;
    private Vector3 _velocity;
    private bool _travelComplete = false;
    private float _finalTravel = 0f;

    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        base.Initialize(spell, payload);

        CollisionLayer = 0;
        CollisionMask = PhysicsLayers.GetOrMask(
            PhysicsLayers.WORLD,
            PhysicsLayers.ENTITY_HURTBOX
        );

        _direction = GlobalPosition.DirectionTo(_payload.TargetPosition);
        _velocity = _direction * Speed;

        LookAt(GlobalPosition + _direction);

        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
    }

    public override void Process(float delta)
    {
        float _gravityForce = _velocity.Y - GravityStrength * delta;
        _velocity = _velocity with { Y = _gravityForce };
        _spell.GlobalPosition += _velocity * delta;

        if (_velocity.LengthSquared() > 0.001)
            _spell.LookAt(_spell.GlobalPosition + _velocity, Vector3.Up);

        if (!_travelComplete)
            return;

        _finalTravel += _velocity.Length();

        if (_finalTravel >= AdditionalTravelAfterCollision)
            RaiseComplete(_payload);
    }

    public void OnBodyEntered(Node3D body)
    {
        _payload.TargetNodes.Add(body);
        BodyEntered -= OnBodyEntered;
        AreaEntered -= OnAreaEntered;
        _travelComplete = true;
    }

    public void OnAreaEntered(Area3D area)
    {
        _payload.TargetNodes.Add(area);
        AreaEntered -= OnAreaEntered;
        BodyEntered -= OnBodyEntered;
        _travelComplete = true;
    }
}