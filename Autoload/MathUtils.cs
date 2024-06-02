using Godot;

namespace Threadcutter.Autoload;

public partial class MathUtils : Node
{
    public static Vector2 Lerp(Vector2 first, Vector2 second, float factor)
    {
        float x = Mathf.Lerp(first.X, second.X, factor);
        float y = Mathf.Lerp(first.Y, second.Y, factor);

        return new Vector2(x, y);
    }
}