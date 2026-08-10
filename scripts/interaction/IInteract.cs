
using Godot;

public interface IInteract
{
    void IsLookedAwayFrom();
    void IsLookedAt();
    void Interact(Node3D interactor, bool held = false);
    bool HasHandler { get; }
    bool IsAvailableTo(Node3D interactor) => true;
}