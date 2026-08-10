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
        for (int i = 0; i < Circle.NumElements; i++)
        {
            RitualElement current = Circle.RitualElements[i];
            current.Position = new Vector2(
                radius * Mathf.Cos(angleChange * i + angleOffset),
                radius * Mathf.Sin(angleChange * i + angleOffset)
            );
            current.Rotation = i * angleChange + angleOffset + Circle.SymbolRotation;
        }
    }
}