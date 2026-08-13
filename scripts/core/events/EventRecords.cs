
using Godot;

public readonly record struct ItemSold(string ItemId, int Amount);
public readonly record struct RatThrown();
public readonly record struct RatLanded();
public readonly record struct CameraImpact(float Charge, float Duration);
public readonly record struct CameraCharge(float Duration, float Delay = 0);
public readonly record struct CameraChargeReset();
public readonly record struct ObjectPlaced();
public readonly record struct NavigationRegionReady(NavigationRegion3D Region);
public readonly record struct ResourceChanged(Economy Type, int OldVal, int NewVal);
public readonly record struct Sundown(int Day);
public readonly record struct DayStarted(int Day);
public readonly record struct SpawnRat(int Amount);
public readonly record struct QuotaUpdated(int Current, int Required);
public readonly record struct ClockTick(string Text, int Day, float DayProgress);