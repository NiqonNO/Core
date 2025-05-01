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

        public static Vector2 ClosestPointOnLine(Vector2 position, Vector2 firstCoordinate, Vector2 secondCoordinate)
        {
            Vector2 pa = position - firstCoordinate, ba = secondCoordinate - firstCoordinate;
            float h = Mathf.Clamp01(Vector2.Dot(pa, ba) / Vector2.Dot(ba, ba));
            return firstCoordinate + h * ba;
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

        public static Vector3 NormaliseBarycentric(Vector3 barycentric)
        {
            barycentric.x = Mathf.Clamp01(barycentric.x);
            barycentric.y = Mathf.Clamp01(barycentric.y);
            barycentric.z = Mathf.Clamp01(barycentric.z);

            float sum = barycentric.x + barycentric.y + barycentric.z;
            if (sum == 0)
                return Vector3.one / 3f;
            return barycentric / sum;
        }

        public static Vector3 InverseLerpBarycentric(float minValue, float maxValue, Vector3 barycentric)
        {
            return NormaliseBarycentric(new Vector3(
                Mathf.InverseLerp(minValue, maxValue, barycentric.x),
                Mathf.InverseLerp(minValue, maxValue, barycentric.y),
                Mathf.InverseLerp(minValue, maxValue, barycentric.z)));
        }

        public static Vector3 LerpBarycentric(float minValue, float maxValue, Vector3 barycentric)
        {
            barycentric = NormaliseBarycentric(barycentric);
            return new Vector3(
                Mathf.Lerp(minValue, maxValue, barycentric.x),
                Mathf.Lerp(minValue, maxValue, barycentric.y),
                Mathf.Lerp(minValue, maxValue, barycentric.z));
        }

        public static Vector3 ClampBarycentric(float minValue, float maxValue, Vector3 barycentric)
        {
            barycentric = InverseLerpBarycentric(minValue, maxValue, barycentric);
            return LerpBarycentric(minValue, maxValue, NormaliseBarycentric(barycentric));
        }

        public static Vector3 GetBarycentricCoordinates(Vector2 position, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            Vector2 v0 = p1 - p0;
            Vector2 v1 = p2 - p0;
            Vector2 v2 = position - p0;

            float d00 = Vector2.Dot(v0, v0);
            float d01 = Vector2.Dot(v0, v1);
            float d11 = Vector2.Dot(v1, v1);
            float d20 = Vector2.Dot(v2, v0);
            float d21 = Vector2.Dot(v2, v1);

            float denom = d00 * d11 - d01 * d01;
            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            if (u < 0)
            {
                float t = Vector2.Dot(position - p1, p2 - p1) / Vector2.Dot(p2 - p1, p2 - p1);
                t = Mathf.Clamp01(t);
                return new Vector3(0.0f, 1.0f - t, t);
            }

            if (v < 0)
            {
                float t = Vector2.Dot(position - p2, p0 - p2) / Vector2.Dot(p0 - p2, p0 - p2);
                t = Mathf.Clamp01(t);
                return new Vector3(t, 0.0f, 1.0f - t);
            }

            if (w < 0)
            {
                float t = Vector2.Dot(position - p0, p1 - p0) / Vector2.Dot(p1 - p0, p1 - p0);
                t = Mathf.Clamp01(t);
                return new Vector3(1.0f - t, t, 0.0f);
            }

            return new Vector3(u, v, w);
        }

        public static Vector3 GetPositionFromBarycentric(Vector3 value, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return value.x * p0 + value.y * p1 + value.z * p2;
        }
        public static Vector3 RoundBarycentric(Vector3 barycentric)
        {
            Vector3Int floored = new Vector3Int(
                Mathf.FloorToInt(barycentric.x),
                Mathf.FloorToInt(barycentric.y),
                Mathf.FloorToInt(barycentric.z)
            );

            int targetSum = Mathf.RoundToInt(barycentric.x + barycentric.y + barycentric.z);
            int currentSum = floored.x + floored.y + floored.z;
            int deficit = targetSum - currentSum;

            if (deficit == 0)
                return floored;

            float fracX = barycentric.x - floored.x;
            float fracY = barycentric.y - floored.y;
            float fracZ = barycentric.z - floored.z;

            for (int i = 0; i < deficit; i++)
            {
                if (fracY >= fracX && fracY >= fracZ)
                {
                    floored.x += 1;
                    fracX = -1f;
                }
                else if (fracX >= fracY && fracX >= fracZ)
                {
                    floored.y += 1;
                    fracY = -1f;
                }
                else
                {
                    floored.z += 1;
                    fracZ = -1f;
                }
            }

            return floored;
        }
    }
}