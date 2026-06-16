using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public struct NOCylindricalBounds : IEquatable<NOCylindricalBounds>
    {
        public float Height;
        public float Radius;
        public Vector3 Center;

        public float HalfHeight => Height * 0.5f;

        public NOCylindricalBounds(float height, float radius, Vector3 center)
        {
            Height = height;
            Radius = radius;
            Center = center;
        }

        public bool Contains(Vector3 point)
        {
            float dx = point.x - Center.x;
            float dz = point.z - Center.z;

            float radialDistanceSquared = (dx * dx) + (dz * dz);

            if (radialDistanceSquared > Radius * Radius)
                return false;

            float minY = Center.y - HalfHeight;
            float maxY = Center.y + HalfHeight;

            return point.y >= minY && point.y <= maxY;
        }

        public bool Intersects(NOCylindricalBounds other)
        {
            float dx = other.Center.x - Center.x;
            float dz = other.Center.z - Center.z;

            float combinedRadius = Radius + other.Radius;

            bool radialOverlap =
                (dx * dx) + (dz * dz) <= combinedRadius * combinedRadius;

            if (!radialOverlap)
                return false;

            float thisMinY = Center.y - HalfHeight;
            float thisMaxY = Center.y + HalfHeight;

            float otherMinY = other.Center.y - other.HalfHeight;
            float otherMaxY = other.Center.y + other.HalfHeight;

            return thisMinY <= otherMaxY && thisMaxY >= otherMinY;
        }

        public override string ToString()
        {
            return $"Center: {Center}, Radius: {Radius}, Height: {Height}";
        }

        public bool Equals(NOCylindricalBounds other)
        {
            return Height.Equals(other.Height)
                   && Radius.Equals(other.Radius)
                   && Center.Equals(other.Center);
        }

        public override bool Equals(object obj)
        {
            return obj is NOCylindricalBounds other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Height, Radius, Center);
        }

        public static bool operator ==(NOCylindricalBounds left, NOCylindricalBounds right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NOCylindricalBounds left, NOCylindricalBounds right)
        {
            return !left.Equals(right);
        }
    }
}