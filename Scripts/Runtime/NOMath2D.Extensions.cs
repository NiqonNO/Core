using UnityEngine;

public static partial class NOMath2D
{
    public static Vector2 Abs(this Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}
