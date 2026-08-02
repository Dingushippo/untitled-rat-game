
using Godot;

public interface IInteract
{
    void IsLookedAwayFrom();
    void IsLookedAt();
    void Interact(Node3D interactor);

    bool HasHandler { get; }
}