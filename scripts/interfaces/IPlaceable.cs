using Godot;
using Godot.NativeInterop;

public interface IPlaceable
{
    Vector3[] SnapPoints {get; set;}
}