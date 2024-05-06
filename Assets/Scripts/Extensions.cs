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
    }
}