using Godot;
using System;

public interface ISpellComponent
{
    public void Initialize(Node3D spell, SpellPayload payload);
    public void Process(float delta);
    public SpellPayload Payload { get; set; }
    public Action<SpellPayload> OnComplete { get; set; }
}