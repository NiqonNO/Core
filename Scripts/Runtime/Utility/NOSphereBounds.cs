using System;
using UnityEngine;

namespace NiqonNO.Core
{
    [Serializable]
    public struct NOSphereBounds : IEquatable<NOSphereBounds>
    {
        public float Radius;
        public Vector3 Center;

        public NOSphereBounds(float radius, Vector3 center)
        {
            Radius = radius;
            Center = center;
        }

        public bool Contains(Vector3 point)
        {
            return Vector3.Distance(Center, point) <= Radius;
        }

        public bool Intersects(NOSphereBounds other)
        {
            return Vector3.Distance(Center, other.Center) <= Radius + other.Radius;
        }

        public override string ToString()
        {
            return $"Center: {Center}, Radius: {Radius}";
        }

        public bool Equals(NOSphereBounds other)
        {
            return Radius.Equals(other.Radius)
                   && Center.Equals(other.Center);
        }

        public override bool Equals(object obj)
        {
            return obj is NOCylindricalBounds other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Radius, Center);
        }

        public static bool operator ==(NOSphereBounds left, NOSphereBounds right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NOSphereBounds left, NOSphereBounds right)
        {
            return !left.Equals(right);
        }
    }
}