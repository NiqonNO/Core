using System;

using UnityEngine;

[Serializable]
public struct NOBarycentricVector3
{
    public const float Sum = 1f;

    public float x;
    public float y;
    public float z;

    public static NOBarycentricVector3 Identity = new(Sum/3f, Sum/3f, Sum/3f);

    public NOBarycentricVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static NOBarycentricVector3 FromX(float x)
    {
        var remaining = (Sum - x) * 0.5f;
        return new NOBarycentricVector3(x, remaining, remaining);
    }

    public static NOBarycentricVector3 FromY(float y)
    {
        var remaining = (Sum - y) * 0.5f;
        return new NOBarycentricVector3(remaining, y, remaining);
    }

    public static NOBarycentricVector3 FromZ(float z)
    {
        var remaining = (Sum - z) * 0.5f;
        return new NOBarycentricVector3(remaining, remaining, z);
    }

    public static NOBarycentricVector3 FromXY(float x, float y)
    {
        return new NOBarycentricVector3(x, y, Sum - x - y);
    }

    public static NOBarycentricVector3 FromXZ(float x, float z)
    {
        return new NOBarycentricVector3(x,  Sum - x - z, z);
    }

    public static NOBarycentricVector3 FromYZ(float y, float z)
    {
        return new NOBarycentricVector3(Sum - y - z, y, z);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public static NOBarycentricVector3 FromVector3(Vector3 v)
    {
        return new NOBarycentricVector3(v.x, v.y, v.z);
    }

    public NOBarycentricVector3 Normalized()
    {
        float sum = x + y + z;
        if (sum == 0f) return NOBarycentricVector3.Identity;
        return new NOBarycentricVector3(x = x / sum, y = y / sum, z = z / sum);
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }

    public static implicit operator Vector3(NOBarycentricVector3 b)
    {
        return new Vector3(b.x, b.y, b.z);
    }

    public static implicit operator NOBarycentricVector3(Vector3 v)
    {
        return new NOBarycentricVector3(v.x, v.y, v.z);
    }
}