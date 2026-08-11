using Godot;
using Godot.Collections;
using System;
using System.Linq;


[GlobalClass]
public partial class HazardManager : Node
{
    [Export] Array<HazardResource> HazardResources;
    [Export] FacilityManager FacilityManager;
    private Dictionary<string, HazardResource> _hazards;
    private Array<FacilityBase> _currentAffectedFacilities = new();
    private Array<Marker3D> _spawnPositions = new();

    public override void _Ready()
    {
        _hazards = new Dictionary<string, HazardResource>(HazardResources.ToDictionary(x => x.Id));

        foreach (Node child in GetChildren())
        {
            if (child is Marker3D marker)
                _spawnPositions.Add(marker);
        }

        EventBus.Subscribe(Event.SpawnHazard, OnSpawnHazard);
    }

    private void OnSpawnHazard(object[] args)
    {
        string hazardId = (string)args[0];
        HazardResource resource = _hazards[hazardId];
        if (!TryGetSpawnMarker(resource, out Marker3D marker))
        {
            GD.Print("Failed to get good position");
            return;
        }
        // Vector3 spawnLocation = newPos;
        Node3D hazardNode = resource.Scene.Instantiate<Node3D>();
        AddChild(hazardNode);
        hazardNode.GlobalTransform = marker.GlobalTransform;
    }
    private bool TryGetSpawnMarker(HazardResource hazard, out Marker3D marker)
    {
        marker = null;
        switch (hazard.SpawnType)
        {
            case HazardSpawnType.NearFacility:
                marker = GetNearFacilitySpawn(); break;
            case HazardSpawnType.OnFloor:
                marker = GetOnFloorSpawn(); break;
            case HazardSpawnType.OnWall:
                marker = GetOnWallSpawn(); break;
            case HazardSpawnType.InSky:
                marker = GetInSkySpawn(); break;
        }
        return marker != null;
    }
    private Marker3D GetNearFacilitySpawn()
    {
        Marker3D marker = _spawnPositions.PickRandom();
        return marker;
    }

    private Marker3D GetOnFloorSpawn()
    {
        throw new NotImplementedException();
    }

    private Marker3D GetOnWallSpawn()
    {
        throw new NotImplementedException();
    }

    private Marker3D GetInSkySpawn()
    {
        throw new NotImplementedException();
    }
}
