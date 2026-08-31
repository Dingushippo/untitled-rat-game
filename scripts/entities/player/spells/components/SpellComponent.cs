using Godot;
using Godot.Collections;


[GlobalClass]
public abstract partial class SpellComponent : Node
{
    public abstract void Execute(SpellPayload payload);

}


public class SpellPayload
{
    public Node3D Caster;
    public Vector3 TargetPosition;
    public int SpellLevel = 1;
    public Array<Node3D> TargetNodes = [];

    public SpellPayload(Node3D caster, Vector3 targetPosition)
    {
        Caster = caster;
        TargetPosition = targetPosition;
    }
}