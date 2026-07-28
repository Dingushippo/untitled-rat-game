using Godot;

namespace Extensions
{
    public static class NodeExtensions
    {
        public static void ClearChildren(this Node node, bool free = true)
        {
            foreach (Node child in node.GetChildren())
        {
            if (free) child.QueueFree();
            
            node.RemoveChild(child);
        }
        }
    }
}