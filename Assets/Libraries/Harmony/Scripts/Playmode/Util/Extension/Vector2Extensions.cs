using System;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Vector2.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Floor this vector.
        /// </summary>
        public static Vector2 AsFloored(this Vector2 value)
        {
            return new Vector2((int) value.x, (int) value.y);
        }

        /// <summary>
        /// Rounds this vector to the closets int.
        /// </summary>
        public static Vector2 AsRounded(this Vector2 value)
        {
            return new Vector2(value.x.RoundToInt(), value.y.RoundToInt());
        }

        /// <summary>
        /// Returns the direction of point1 to point2.
        /// </summary>
        public static Vector2 DirectionTo(this Vector2 point1, Vector2 point2)
        {
            return point2 - point1;
        }

        /// <summary>
        /// Compute distance between two points.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        /// <returns>Distance between the two points.</returns>
        public static float DistanceTo(this Vector2 point1, Vector2 point2)
        {
            return (point2 - point1).magnitude;
        }

        /// <summary>
        /// Compute distance between two points, squared.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        /// <returns>Squared distance between the deux points.</returns>
        public static float SqrDistanceTo(this Vector2 point1, Vector2 point2)
        {
            return (point2 - point1).sqrMagnitude;
        }

        /// <summary>
        /// Tells if a point is near an other.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        /// <param name="maxDistance">Max distance.</param>
        /// <returns>True if "Point1" is near "Point2", false otherwise.</returns>
        public static bool IsCloseOf(this Vector2 point1, Vector2 point2, float maxDistance)
        {
            if (maxDistance < 0)
                throw new ArgumentException(nameof(maxDistance) + " must be greater or equal to 0.");

            return point1.SqrDistanceTo(point2) < maxDistance * maxDistance;
        }

        /// <summary>
        /// Tells if two vectors are in the same direction.
        /// </summary>
        /// <param name="direction1">Direction 1.</param>
        /// <param name="direction2">Direction 2.</param>
        /// <param name="precision">
        /// Precision. Using 0, the maximum angle is 90 degrees. Using 1, the maximum angle is 0 degrees.
        /// This value is usually the maximum angle cosine.
        /// </param>
        /// <returns>True is "Direction1" points the same way as "Direction2", false otherwise.</returns>
        public static bool IsSameDirection(this Vector2 direction1, Vector2 direction2, float precision = 1f)
        {
            if (precision < 0 || precision > 1)
            {
                throw new ArgumentException("Precision must be between 0 and 1, inclusive.");
            }

            return Vector2.Dot(direction1.normalized, direction2.normalized) >= precision;
        }

        /// <summary>
        /// Find the closest point on a line from an other point.
        /// </summary>
        /// <param name="from">Line start position.</param>
        /// <param name="to">Line end position.</param>
        /// <param name="position">Position.</param>
        /// <returns>Nearest point on the line.</returns>
        public static Vector2 ClosestPointOnLine(Vector2 from, Vector2 to, Vector2 position)
        {
            var directionToLine = position - from;
            var lineDirectionNormalized = (to - from).normalized;

            var lineLength = Vector2.Distance(from, to);
            var travelDistance = Vector2.Dot(directionToLine, lineDirectionNormalized);

            if (travelDistance <= 0) return from;
            if (travelDistance >= lineLength) return to;

            var travelDirection = lineDirectionNormalized * travelDistance;
            var closestPoint = from + travelDirection;

            return closestPoint;
        }

        /// <summary>
        /// Rotates the point.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>Rotated point.</returns>
        public static Vector2 Rotate(this Vector2 point, float angle)
        {
            angle = Mathf.Deg2Rad * angle;

            float angleCos = Mathf.Cos(angle);
            float angleSin = Mathf.Sin(angle);

            return new Vector2(
                point.x * angleCos - point.y * angleSin,
                point.x * angleSin + point.y * angleCos
            );
        }

        /// <summary>
        /// Rotates the point around a pivot.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="pivot">Pivot.</param>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>Rotated point.</returns>
        public static Vector2 RotateAround(this Vector2 point, Vector2 pivot, float angle)
        {
            return (point - pivot).Rotate(angle) + pivot;
        }
    }
}