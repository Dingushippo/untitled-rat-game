using System;
using Godot;


[GlobalClass]
public partial class InteractAreaComponent : Area3D, IInteract
{
    public const uint INTERACT_LAYER = 4;

    [Export] public string InteractionText = "Interact";
    [Export] public bool ShowInteractionText = true;
    [Export] public Vector3 InteractionTextOffset = new Vector3(0, 2, 0);
    [Export] public bool IsEnabled = true;
    public Action<Node3D> OnInteract;

    /// <summary>True when something is actually listening, so callers can tell a live prompt from scenery.</summary>
    public bool HasHandler => IsEnabled && OnInteract != null;

    private Label3D interactionLabel;
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
        if (!active) IsLookedAwayFrom();
    }

    private void InstantiateInteractionLabel()
    {
        interactionLabel = new Label3D
        {
            Visible = false,
            Text = InteractionText,
            Billboard = BaseMaterial3D.BillboardModeEnum.FixedY,
            Transform = new Transform3D(Basis.Identity, InteractionTextOffset),
        };
        AddChild(interactionLabel);
    }
    public void IsLookedAt()
    {
        if (!IsEnabled) return;
        // Show interaction text above the object
        if (ShowInteractionText)
        {
            interactionLabel.Visible = true;
        }
    }

    public void IsLookedAwayFrom()
    {
        if (interactionLabel is not null) interactionLabel.Visible = false;
    }

    public void Interact(Node3D interactor)
    {
        if (!IsEnabled) return;
        OnInteract?.Invoke(interactor);
    }

    /// <summary>Updates the floating prompt, so facilities can advertise what is collectable.</summary>
    public void SetInteractionText(string text)
    {
        InteractionText = text;
        if (interactionLabel is not null) interactionLabel.Text = text;
    }
}