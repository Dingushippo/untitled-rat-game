
using System.Runtime.InteropServices;

public readonly record struct ItemSold(string ItemId, int Amount);
public readonly record struct RatThrown();
public readonly record struct RatLanded();
public readonly record struct CameraImpact(float Charge, float Duration);