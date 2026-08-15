using Godot;
using System;

public enum HandRequirement
{
    Any,
    Empty,
    HoldsRat,
}

[GlobalClass]
public partial class InteractAreaComponent : Area3D, IInteract
{
    public const uint INTERACT_LAYER = 4;

    [Export]
    public string InteractionText = "Interact";

    [Export]
    public bool ShowInteractionText = true;

    [Export]
    public Vector3 InteractionTextOffset = new Vector3(0, 2, 0);

    [Export]
    public bool IsEnabled = true;

    [Export]
    public HandRequirement RequiredHands = HandRequirement.Any;
    public Action<Node3D, bool> OnInteract;
    public Action OnLookedAt;
    public Action OnLookedAwayFrom;

    /// <summary>True when something is actually listening, so callers can tell a live prompt from scenery.</summary>
    public bool HasHandler => IsEnabled && OnInteract != null;

    private Label3D _interactionLabel;

    public override void _Ready()
    {
        CollisionLayer = IsEnabled ? INTERACT_LAYER : 0;
        CollisionMask = 0;
        InstantiateInteractionLabel();
    }

    /// <summary>
    /// Drops the area off the interact layer entirely. A held rat sits in front of the camera,
    /// so leaving it detectable would swallow every raycast aimed past it.
    /// </summary>
    public void SetActive(bool active)
    {
        IsEnabled = active;
        CollisionLayer = active ? INTERACT_LAYER : 0;
        if (!active)
            IsLookedAwayFrom();
    }

    private void InstantiateInteractionLabel()
    {
        _interactionLabel = new Label3D
        {
            Visible = false,
            Text = InteractionText,
            Billboard = BaseMaterial3D.BillboardModeEnum.FixedY,
            Transform = new Transform3D(Basis.Identity, InteractionTextOffset),
        };
        AddChild(_interactionLabel);
    }

    public void IsLookedAt()
    {
        if (!IsEnabled)
            return;
        // Show interaction text above the object
        if (ShowInteractionText)
        {
            _interactionLabel.Visible = true;
        }
        OnLookedAt?.Invoke();
    }

    public void IsLookedAwayFrom()
    {
        if (_interactionLabel is null)
            return;

        _interactionLabel.Visible = false;
        OnLookedAwayFrom?.Invoke();
    }

    public void Interact(Node3D interactor, bool isHeld)
    {
        if (!IsEnabled)
            return;
        OnInteract?.Invoke(interactor, isHeld);
    }

    public bool IsAvailableTo(Node3D interactor)
    {
        if (interactor is not Player player)
            return false;
        if (RequiredHands == HandRequirement.Any)
            return true;
        bool hasGrab = player.GrabComponent.HasGrabbed();
        if (RequiredHands == HandRequirement.HoldsRat && hasGrab)
            return true;
        if (RequiredHands == HandRequirement.Empty && !hasGrab)
            return true;
        return false;
    }

    /// <summary>Updates the floating prompt, so facilities can advertise what is collectable.</summary>
    public void SetInteractionText(string text)
    {
        InteractionText = text;
        if (_interactionLabel is not null)
            _interactionLabel.Text = text;
    }
}