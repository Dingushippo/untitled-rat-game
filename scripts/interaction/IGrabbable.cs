using System;
using Godot;

public interface IGrabbable
{
    void Grab();
    void Release();
    Vector3 GrabOrientation {get; }
    Vector3 GrabOffset {get;}
}