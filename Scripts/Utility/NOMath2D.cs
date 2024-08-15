using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.Core.Utility
{
    public static partial class NOMath2D
    {
        public static float DistanceToPolygon(Vector2 position, IList<Vector2> coordinates)
        {
            float d = Vector2.Dot(position - coordinates[0], position - coordinates[0]);
            float s = 1.0f;
            for (int i = 0, j = coordinates.Count - 1; i < coordinates.Count; j = i, i++)
            {
                Vector2 e = coordinates[j] - coordinates[i];
                Vector2 w = position - coordinates[i];
                Vector2 b = w - e * Mathf.Clamp01(Vector2.Dot(w, e) / Vector2.Dot(e, e));
                d = Mathf.Min(d, Vector2.Dot(b, b));
                var condA = position.y >= coordinates[i].y;
                var condB = position.y < coordinates[j].y;
                var condC = e.x * w.y > e.y * w.x;
                if ((condA && condB && condC) || (!condA && !condB && !condC)) s *= -1.0f;
            }
            return s * Mathf.Sqrt(d);
        }
        public static float AreaOfPolygon(IList<Vector2> coordinates)
        {
            float area = 0f;
            for (int i = 0; i < coordinates.Count; i++)
            {
                Vector2 current = coordinates[i];
                Vector2 next = coordinates[(i + 1) % coordinates.Count];
                area += current.x * next.y - next.x * current.y;
            }
            area *= 0.5f;
            return Mathf.Abs(area);
        }

        public static float DistanceToPath(Vector2 position, IList<Vector2> coordinates)
        {
            float s = float.MaxValue;
            for (int i = 1, j = 0; i < coordinates.Count; j = i, i++)
            {
                s = Mathf.Min(DistanceToLine(position, coordinates[j], coordinates[i]), s);
            }
            return s;
        }
        public static float DistanceToLine(Vector2 position, Vector2 firstCoordinate, Vector2 secondCoordinate)
        {
            Vector2 pa = position - firstCoordinate, ba = secondCoordinate - firstCoordinate;
            float h = Mathf.Clamp01(Vector2.Dot(pa, ba) / Vector2.Dot(ba, ba));
            return (pa - ba * h).magnitude;
        }
        public static float LengthOfPath(IList<Vector2> coordinates)
        {
            {
                float length = 0f;
                for (int i = 0; i < coordinates.Count - 1; i++)
                {
                    length += Vector2.Distance(coordinates[i], coordinates[i + 1]);
                }
                return length;
            }
        }

        public static float DistanceToBox(Vector2 position, Rect coordinates)
        {
            position -= coordinates.position;
            Vector2 d = position.Abs() - coordinates.size;
            return Vector2.Max(d, Vector2.zero).magnitude + Mathf.Min(Mathf.Max(d.x, d.y), 0.0f);
        }

        public static float DistanceToPoint(Vector2 position, Vector2 coordinates)
        {
            return (position - coordinates).magnitude;
        }
    }
}