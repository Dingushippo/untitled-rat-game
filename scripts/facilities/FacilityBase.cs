using Godot;
using System.Linq;


public abstract partial class FacilityBase : StaticBody3D, ICatchArea
{

    [Export] public Marker3D IntakeMarker;
    [Export] public FacilityDef Facility;
    [Export] public Label3D DebugLabel;

    /// <summary>
    /// World Y of the top of this facility's collision shapes. Homing throws lift their approach
    /// above this so the curve arcs over the structure instead of clipping through it.
    /// </summary>
    public float ColliderTopY { get; set; }

    public override void _Ready()
    {
        if (Facility is null)
        {
            GD.PushWarning($"{Name} is missing a facility definition");
            SetProcess(false);
            return;
        }

        ColliderTopY = ComputeColliderTopY();
    }

    protected abstract bool WantsCargo(Rat rat);

    public abstract Godot.Collections.Dictionary<string, int> DeliverCargo(Rat rat);

    public virtual bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target)
    {
        target = default;
        if (rat is null) return false;

        if (IntakeMarker is not null && WantsCargo(rat))
        {
            target = ThrowTarget.Intake(IntakeMarker, this);
            return true;
        }
        return false;
    }

    private float ComputeColliderTopY()
    {
        float top = GlobalPosition.Y;

        foreach (Node child in GetChildren())
        {
            if (child is not CollisionShape3D shapeNode || shapeNode.Disabled || shapeNode.Shape is null)
                continue;

            Mesh debugMesh = shapeNode.Shape.GetDebugMesh();
            if (debugMesh is null) continue;

            Aabb bounds = shapeNode.GlobalTransform * debugMesh.GetAabb();
            top = Mathf.Max(top, bounds.End.Y);
        }

        return top;
    }
    protected virtual void UpdateDebugLabel() { }
}
