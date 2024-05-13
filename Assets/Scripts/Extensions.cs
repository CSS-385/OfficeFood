using UnityEngine;

namespace OfficeFood
{
    public static class Extensions
    {
        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
                v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle)
            );
        }

        public static bool IsCollinear(this Vector2 point, Vector2 pointA, Vector2 pointB, float threshold)
        {
            float distAB = Vector2.Distance(pointA, pointB);
            float distAP = Vector2.Distance(pointA, point);
            float distBP = Vector2.Distance(pointB, point);
            return Mathf.Abs(distAP + distBP - distAB) < threshold;
        }

        public static bool PerpendicularToSegment(this Vector2 point, Vector2 segmentA, Vector2 segmentB)
        {
            Vector2 ab = segmentB - segmentA;
            Vector2 ap = point - segmentA;
            Vector2 bp = point - segmentB;
            return !(Vector2.Dot(ab, ap) < 0.0f || Vector2.Dot(ab, bp) > 0.0f);
        }

        public static float DistanceToSegment(this Vector2 point, Vector2 segmentA, Vector2 segmentB)
        {
            if (segmentA == segmentB)
            {
                // Note: Vector2== checks if approximately equal.
                return Vector2.Distance(point, segmentA);
            }

            Vector2 ab = segmentB - segmentA;
            Vector2 ap = point - segmentA;
            Vector2 bp = point - segmentB;

            if (Vector2.Dot(ab, ap) < 0.0f)
            {
                // Point->A points in the opposite direction as A->B, therefore A is the closest point.
                return Vector2.Distance(point, segmentA);
            }

            if (Vector2.Dot(ab, bp) > 0.0f)
            {
                // Point->B points in the same direction as A->B, therefore B is the closest point.
                return Vector2.Distance(point, segmentB);
            }

            // The point is perpendicular to the A->B line segment, therefore find distance using cross product.
            return Mathf.Abs(ab.x * ap.y - ab.y * bp.x) / ab.magnitude;
        }
    }
}
