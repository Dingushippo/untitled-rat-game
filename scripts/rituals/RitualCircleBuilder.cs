using Godot;
public class RitualCircleBuilder
{
    public RitualCircleResource Circle;


    public void AddElement(RitualElement element, int position = -1)
    {
        Circle.RitualElements.Insert(position, element);
        RepositionElements();
    }

    public void SetCircle(RitualCircleResource circle)
    {
        GD.Print("Set circle");
        Circle = circle;
        RepositionElements();
    }

    public void RemoveElement(int position)
    {
        Circle.RitualElements.RemoveAt(position);
        RepositionElements();
    }

    public void RepositionElements()
    {

        float angleChange = Mathf.Tau / Circle.NumElements;
        float angleOffset = Circle.AngleOffset;
        float radius = Circle.Radius;
        GD.Print($"Attempting reposition of {Circle.NumElements} elements:\nAngleChange: {angleChange}\nAngleOffset: {angleOffset}\nRadius: {radius}");
        for (int i = 0; i < Circle.NumElements; i++)
        {
            RitualElement current = Circle.RitualElements[i];
            current.Position = new Vector2(
                radius * Mathf.Cos(angleChange * i + angleOffset),
                radius * Mathf.Sin(angleChange * i + angleOffset)
            );
            current.Rotation = i * angleChange + angleOffset + Circle.SymbolRotation;
            GD.Print($"Set position: {current.Position}, rotation: {current.Rotation}");
        }
    }
}