using System;
using Godot;


[GlobalClass]
public partial class InteractComponent : Area3D
{
    [Export] public string InteractionText = "Interact";
    [Export] public bool ShowInteractionText = true;
    [Export] public Vector3 InteractionTextOffset = new Vector3(0, 2, 0);
    [Export] public bool IsEnabled = true;
    public Action OnInteract;

    private Label3D interactionLabel;
    public override void _Ready()
    {
        CollisionLayer = 4;
        CollisionMask = 0;
        InstantiateInteractionLabel();
    }

    private void InstantiateInteractionLabel()
    {        
        interactionLabel = new Label3D
        {
            Visible = false,
            Text = InteractionText,
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
            Transform = new Transform3D(Basis.Identity, InteractionTextOffset),
        };
        AddChild(interactionLabel);
    }
    public void IsLookedAt()
    {
        if (!IsEnabled) return;
        GD.Print("Looking at " + GetParent().Name);
        // Show interaction text above the object
        if (ShowInteractionText)
        {
            interactionLabel.Visible = true;
        }
    }

    public void IsLookedAwayFrom()
    {
        GD.Print("Looking away from " + GetParent().Name);
        interactionLabel.Visible = false;
    }

    public void Interact()
    {
        if (!IsEnabled) return;

        GD.Print("Interacted with " + GetParent().Name);
        OnInteract?.Invoke();
    }
}