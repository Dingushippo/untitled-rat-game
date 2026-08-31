using Godot;
using Godot.Collections;
public static class ArrayExtensions
{
    public static T PopAt<[MustBeVariant] T>(this Array<T> array, int index)
    {
        var item = array[index];
        array.RemoveAt(index);
        return item;
    }
}
