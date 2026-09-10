using Godot;
using System;

public abstract partial class Area3DSpellComponent : Area3D, ISpellComponent
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

public abstract partial class TrailSpellComponent : Trail3D, ISpellComponent
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

public abstract partial class TimerSpellComponent : Timer, ISpellComponent
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
