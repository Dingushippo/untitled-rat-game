using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RayTargetingComponent : BaseTargetingComponent
{
    [Export] public float Range = 50f;
    protected override void AcquireTarget()
    {
        Vector3 origin = _payload.Player.Camera.GlobalPosition;
        Vector3 target = origin - _payload.Player.Camera.GlobalBasis.Z * Range;

        uint collisionMask = PhysicsLayers.GetOrMask(
            PhysicsLayers.WORLD,
            PhysicsLayers.ENTITY_HURTBOX
        );

        if (RaycastUtils.Ray(_payload.Caster, origin, target, out Dictionary result, collisionMask))
        {
            GD.Print($"Ray hit");
            _payload.TargetPosition = result["position"].AsVector3();
        }
        else
            _payload.TargetPosition = target;
    }
}