using UnityEngine;

namespace NiqonNO.Core.Utility
{
    public static class NOGizmoUtility
    {
        public static void DrawCylinder(Vector3 center, Vector3 normal, float radius, float height, int segments = 32)
        {
            normal = normal.normalized;
            Vector3 up = normal * (height * 0.5f);

            Vector3 topCenter = center + up;
            Vector3 bottomCenter = center - up;

            DrawCircle(topCenter, normal, radius, segments);
            DrawCircle(bottomCenter, normal, radius, segments);

            Camera cam = Camera.current;
            if (cam != null)
            {
                DrawSides(cam.transform.forward);
                return;
            }

            Vector3 tangent = Vector3.Cross(normal, Vector3.up);
            if (tangent.sqrMagnitude < 0.001f) tangent = Vector3.Cross(normal, Vector3.right);
            tangent.Normalize();

            Vector3 bitangent = Vector3.Cross(normal, tangent);

            DrawSides(tangent);
            DrawSides(bitangent);
            DrawSides((tangent + bitangent).normalized);
            DrawSides((tangent - bitangent).normalized);
            return;

            void DrawSides(Vector3 direction)
            {
                Vector3 right = Vector3.Cross(normal, direction).normalized;
                Vector3 side = right * radius;

                Gizmos.DrawLine(topCenter + side, bottomCenter + side);
                Gizmos.DrawLine(topCenter - side, bottomCenter - side);
            }
        }

        public static void DrawCircle(Vector3 center, Vector3 normal, float radius, int segments = 32)
        {
            normal = normal.normalized;

            Vector3 tangent = Vector3.Cross(normal, Vector3.up);
            if (tangent.sqrMagnitude < 0.001f) tangent = Vector3.Cross(normal, Vector3.right);
            tangent.Normalize();

            Vector3 bitangent = Vector3.Cross(normal, tangent);

            float angleStep = Mathf.PI * 2f / segments;

            Vector3 prevPoint = center + tangent * radius;

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i;

                Vector3 localPoint =
                    Mathf.Cos(angle) * tangent * radius +
                    Mathf.Sin(angle) * bitangent * radius;

                Vector3 nextPoint = center + localPoint;

                Gizmos.DrawLine(prevPoint, nextPoint);

                prevPoint = nextPoint;
            }
        }
    }
}