using Godot;
using Godot.Collections;

public partial class SellingFacility : FacilityBase
{
    public override Dictionary<string, int> DeliverCargo(Rat rat)
    {
        var moved = rat.Cargo.RemoveAll();
        foreach (var (item, amount) in moved)
        {
            // EventBus.Publish(Event.ItemSold, item, amount);
            EventBus.Publish(new ItemSold(item, amount));
        }
        return moved;
    }

    protected override bool WantsCargo(Rat rat)
    {
        return !rat.Cargo.IsEmpty;
    }

}
