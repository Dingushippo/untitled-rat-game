# One-shot repo reorganisation. Moves files with git mv (preserving history),
# keeps each .cs.uid with its .cs, then rewrites res:// paths in scenes/resources.
$ErrorActionPreference = 'Stop'
Set-Location 'C:\Users\danbj\Documents\Godot projects\untitled-rat-game'

$map = [ordered]@{
    # --- core infra -------------------------------------------------------
    'scripts/components/FiniteStateMachine.cs' = 'scripts/core/fsm/FiniteStateMachine.cs'
    'scripts/components/State.cs'              = 'scripts/core/fsm/State.cs'
    'scripts/components/ObjectPoolComponent.cs' = 'scripts/core/pooling/ObjectPoolComponent.cs'
    'scripts/interfaces/IPooledObject.cs'      = 'scripts/core/pooling/IPooledObject.cs'
    'scripts/singletons/Event.cs'              = 'scripts/core/events/Event.cs'
    'scripts/singletons/EventBus.cs'           = 'scripts/core/events/EventBus.cs'
    'scripts/singletons/Utils.cs'              = 'scripts/core/Utils.cs'
    'scripts/extensions/NodeExtensions.cs'     = 'scripts/core/NodeExtensions.cs'

    # --- inventory (shared contract: rats <-> facilities) -----------------
    'scripts/components/Inventory.cs'          = 'scripts/inventory/Inventory.cs'
    'scripts/interfaces/IInventory.cs'         = 'scripts/inventory/IInventory.cs'

    # --- facilities -------------------------------------------------------
    'scripts/FacilityBase.cs'                  = 'scripts/facilities/FacilityBase.cs'
    'scripts/WorkSlot.cs'                      = 'scripts/facilities/WorkSlot.cs'
    'scripts/components/ProductionComponent.cs' = 'scripts/facilities/ProductionComponent.cs'
    'scripts/resources/FacilityDef.cs'         = 'scripts/facilities/FacilityDef.cs'

    # --- entities: rat ----------------------------------------------------
    'scripts/entity/rat/Rat.cs'                = 'scripts/entities/rat/Rat.cs'
    'scripts/resources/RatDef.cs'              = 'scripts/entities/rat/RatDef.cs'
    'scripts/resources/RatFlightTuning.cs'     = 'scripts/entities/rat/RatFlightTuning.cs'
    'scripts/entity/rat/states/RatState.cs'        = 'scripts/entities/rat/states/RatState.cs'
    'scripts/entity/rat/states/RatCurveState.cs'   = 'scripts/entities/rat/states/RatCurveState.cs'
    'scripts/entity/rat/states/RatFallingState.cs' = 'scripts/entities/rat/states/RatFallingState.cs'
    'scripts/entity/rat/states/RatFollowState.cs'  = 'scripts/entities/rat/states/RatFollowState.cs'
    'scripts/entity/rat/states/RatGrabState.cs'    = 'scripts/entities/rat/states/RatGrabState.cs'
    'scripts/entity/rat/states/RatIdleState.cs'    = 'scripts/entities/rat/states/RatIdleState.cs'
    'scripts/entity/rat/states/RatIntakeState.cs'  = 'scripts/entities/rat/states/RatIntakeState.cs'
    'scripts/entity/rat/states/RatLandedState.cs'  = 'scripts/entities/rat/states/RatLandedState.cs'
    'scripts/entity/rat/states/RatSlottedState.cs' = 'scripts/entities/rat/states/RatSlottedState.cs'

    # --- entities: player -------------------------------------------------
    'scripts/player/Player.cs'                 = 'scripts/entities/player/Player.cs'
    'scripts/player/PlayerCamera.cs'           = 'scripts/entities/player/PlayerCamera.cs'
    'scripts/player/CrouchComponent.cs'        = 'scripts/entities/player/abilities/CrouchComponent.cs'
    'scripts/player/GrabComponent.cs'          = 'scripts/entities/player/abilities/GrabComponent.cs'
    'scripts/player/InteractComponent.cs'      = 'scripts/entities/player/abilities/InteractComponent.cs'
    'scripts/player/ThrowComponent.cs'         = 'scripts/entities/player/abilities/ThrowComponent.cs'
    'scripts/player/HandController.cs'         = 'scripts/entities/player/abilities/HandController.cs'
    'scripts/player/states/hand/HandState.cs'      = 'scripts/entities/player/states/hand/HandState.cs'
    'scripts/player/states/hand/HandEmptyState.cs' = 'scripts/entities/player/states/hand/HandEmptyState.cs'
    'scripts/player/states/hand/HandGrabState.cs'  = 'scripts/entities/player/states/hand/HandGrabState.cs'
    'scripts/player/states/movement/PlayerState.cs'        = 'scripts/entities/player/states/movement/PlayerState.cs'
    'scripts/player/states/movement/PlayerIdleState.cs'    = 'scripts/entities/player/states/movement/PlayerIdleState.cs'
    'scripts/player/states/movement/PlayerMoveState.cs'    = 'scripts/entities/player/states/movement/PlayerMoveState.cs'
    'scripts/player/states/movement/PlayerJumpState.cs'    = 'scripts/entities/player/states/movement/PlayerJumpState.cs'
    'scripts/player/states/movement/PlayerFallingState.cs' = 'scripts/entities/player/states/movement/PlayerFallingState.cs'
    'scripts/player/states/movement/PlayerSlideState.cs'   = 'scripts/entities/player/states/movement/PlayerSlideState.cs'
    'scripts/player/states/movement/PlayerVaultState.cs'   = 'scripts/entities/player/states/movement/PlayerVaultState.cs'

    # --- throws (ThrowTarget belongs with the throw vocabulary) -----------
    'scripts/player/throws/ThrowType.cs'    = 'scripts/entities/player/throws/ThrowType.cs'
    'scripts/player/throws/SimpleThrow.cs'  = 'scripts/entities/player/throws/SimpleThrow.cs'
    'scripts/player/throws/ThrowPath.cs'    = 'scripts/entities/player/throws/ThrowPath.cs'
    'scripts/player/throws/ThrowContext.cs' = 'scripts/entities/player/throws/ThrowContext.cs'
    'scripts/ThrowTarget.cs'                = 'scripts/entities/player/throws/ThrowTarget.cs'
    'scripts/resources/ThrowTuning.cs'      = 'scripts/entities/player/throws/ThrowTuning.cs'

    # --- interaction ------------------------------------------------------
    'scripts/components/InteractAreaComponent.cs' = 'scripts/interaction/InteractAreaComponent.cs'
    'scripts/interfaces/IGrabbable.cs'            = 'scripts/interaction/IGrabbable.cs'
    'scripts/interfaces/IThrowable.cs'            = 'scripts/interaction/IThrowable.cs'

    # --- placement --------------------------------------------------------
    'scripts/interfaces/PlaceableObject.cs'     = 'scripts/placement/PlaceableObject.cs'
    'scripts/interfaces/IPlaceable.cs'          = 'scripts/placement/IPlaceable.cs'
    'scripts/player/ObjectPlacementRaycast.cs'  = 'scripts/placement/ObjectPlacementRaycast.cs'
    'scripts/resources/ObjectResource.cs'       = 'scripts/placement/ObjectResource.cs'

    # --- world / services / debug ----------------------------------------
    'scripts/NavigationRegion3d.cs'            = 'scripts/world/NavigationRegion3d.cs'
    'scripts/singletons/ObjectManager.cs'      = 'scripts/services/ObjectManager.cs'
    'scripts/singletons/Game/RatThrowTuning.cs' = 'scripts/debug/RatThrowTuning.cs'

    # --- scenes -----------------------------------------------------------
    'scenes/player.tscn'            = 'scenes/entities/player.tscn'
    'scenes/rat.tscn'               = 'scenes/entities/rat.tscn'
    'scenes/hand_controller.tscn'   = 'scenes/entities/hand_controller.tscn'
    'scenes/placeable_object.tscn'  = 'scenes/placement/placeable_object.tscn'
    'scenes/rat_throw_tuning.tscn'  = 'scenes/debug/rat_throw_tuning.tscn'

    # --- resources --------------------------------------------------------
    'resources/object/TestObjectResource.tres'  = 'resources/objects/TestObjectResource.tres'
    'resources/object/TestObjectResource2.tres' = 'resources/objects/TestObjectResource2.tres'
}

Write-Host '=== moving files ===' -ForegroundColor Cyan
foreach ($old in $map.Keys) {
    $new = $map[$old]
    if (-not (Test-Path $old)) { Write-Host "  SKIP (missing): $old" -ForegroundColor Yellow; continue }

    $dir = Split-Path $new -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

    git mv --force $old $new
    if ($LASTEXITCODE -ne 0) { throw "git mv failed: $old -> $new" }

    # A .cs.uid must travel with its script or Godot regenerates the uid and breaks scene refs.
    if (Test-Path "$old.uid") {
        git mv --force "$old.uid" "$new.uid"
        if ($LASTEXITCODE -ne 0) { throw "git mv failed: $old.uid" }
    }
    Write-Host "  $old -> $new"
}

Write-Host '=== rewriting res:// references ===' -ForegroundColor Cyan
$targets = Get-ChildItem -Recurse -Include *.tscn, *.tres, *.godot -File |
    Where-Object { $_.FullName -notmatch '\\\.godot\\|\\\.git\\' }

foreach ($file in $targets) {
    $text = Get-Content $file.FullName -Raw
    $original = $text
    foreach ($old in $map.Keys) {
        $text = $text.Replace("res://$old", "res://" + $map[$old])
    }
    if ($text -ne $original) {
        Set-Content -Path $file.FullName -Value $text -NoNewline
        Write-Host "  updated $($file.FullName.Replace((Get-Location).Path + '\', ''))"
    }
}

Write-Host '=== removing stale csproj backups ===' -ForegroundColor Cyan
Get-ChildItem -Filter '*.csproj.old*' -File | ForEach-Object {
    Write-Host "  delete $($_.Name)"
    Remove-Item $_.FullName -Force
}

Write-Host '=== pruning empty dirs ===' -ForegroundColor Cyan
foreach ($d in @('scripts/components','scripts/entity/rat/states','scripts/entity/rat','scripts/entity',
                 'scripts/extensions','scripts/interfaces','scripts/player/states/hand',
                 'scripts/player/states/movement','scripts/player/states','scripts/player/throws',
                 'scripts/player','scripts/resources','scripts/singletons/Game','scripts/singletons',
                 'resources/object')) {
    if ((Test-Path $d) -and -not (Get-ChildItem $d -Recurse -File)) {
        Remove-Item $d -Recurse -Force
        Write-Host "  removed $d"
    }
}

Write-Host 'DONE' -ForegroundColor Green
