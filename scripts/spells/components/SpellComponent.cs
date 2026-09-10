using Godot;
using Godot.Collections;
using System;


public abstract partial class SpellComponent : Node, ISpellComponent
{
    public event Action<SpellPayload> OnComplete;
    public event Action OnStarted;
    public event Action<float> OnProgressChanged;
    protected Node3D _spell;
    protected SpellPayload _payload;
    public virtual void Initialize(Node3D spell, SpellPayload payload) { _spell = spell; _payload = payload; }
    protected virtual void RaiseComplete(SpellPayload payload) => OnComplete?.Invoke(payload);
    protected virtual void RaiseStarted() => OnStarted?.Invoke();
    protected virtual void RaiseProgressChanged(float progress) => OnProgressChanged?.Invoke(progress);
    public virtual void Process(float delta) { }
    public override string ToString() => GetType().ToString();

    public override void _ExitTree()
    {
        // Clear out invocation lists to prevent memory pins during Hot Reload
        OnComplete = null;
        OnStarted = null;
        OnProgressChanged = null;
        base._ExitTree();
    }
}

public class SpellPayload
{
    public Player Player;
    public Node3D Caster;
    public Vector3 TargetPosition;
    public int SpellLevel = 1;
    public Array<Node3D> TargetNodes = [];
    public SpellPayload(Player player, Node3D caster)
    {
        Player = player;
        Caster = caster;
    }
    public override string ToString()
        => $"Target: {TargetPosition}, level: {SpellLevel}, targets: {string.Join(',', TargetNodes)}";
}

public interface ISpellComponent
{
    event Action<SpellPayload> OnComplete;
    event Action OnStarted;
    event Action<float> OnProgressChanged;
    void Initialize(Node3D spell, SpellPayload payload);
    void Process(float delta);
}