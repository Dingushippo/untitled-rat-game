using Godot;

public partial class RitualSelection : Node
{
	public RitualElement Selected { get; private set; }

	public void Select(RitualElement element)
	{
		Selected = element;
	}

	public void Clear()
	{
		Selected = null;
	}
}