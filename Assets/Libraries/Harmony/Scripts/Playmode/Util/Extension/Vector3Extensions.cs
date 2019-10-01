﻿using System;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Vector3.
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Floor this vector.
        /// </summary>
        public static Vector3 AsFloored(this Vector3 value)
        {
            return new Vector3((int) value.x, (int) value.y, (int) value.z);
        }
        
        /// <summary>
        /// Rounds this vector to the closets int.
        /// </summary>
        public static Vector3 AsRounded(this Vector3 value)
        {
            return new Vector3(value.x.RoundToInt(), value.y.RoundToInt(), value.z.RoundToInt());
        }
        
        /// <summary>
        /// Returns the direction of point1 to point2.
        /// </summary>
        public static Vector3 DirectionTo(this Vector3 point1, Vector3 point2)
        {
            return point2 - point1;
        }
        
        /// <summary>
        /// Compute distance between two points.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        /// <returns>Distance between the two points.</returns>
        public static float DistanceTo(this Vector3 point1, Vector3 point2)
        {
            return (point2 - point1).magnitude;
        }
        
        /// <summary>
        /// Compute distance between two points, squared.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        /// <returns>Squared distance between the deux points.</returns>
        public static float SqrDistanceTo(this Vector3 point1, Vector3 point2)
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
        public static bool IsCloseOf(this Vector3 point1, Vector3 point2, float precision = 0f)
        {
            if (precision < 0)
                throw new ArgumentException("Precision must be greater or equal to 0.");
            
            return point1.SqrDistanceTo(point2) < precision * precision;
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
        public static bool IsSameDirection(this Vector3 direction1, Vector3 direction2, float precision = 1f)
        {
            if (precision < 0 || precision > 1)
                throw new ArgumentException("Precision must be between 0 and 1, inclusive.");
            
            return Vector3.Dot(direction1.normalized, direction2.normalized) >= precision;
        }
        
        /// <summary>
        /// Find the closest point on a line from an other point.
        /// </summary>
        /// <param name="from">Line start position.</param>
        /// <param name="to">Line end position.</param>
        /// <param name="position">Position.</param>
        /// <returns>Nearest point on the line.</returns>
        public static Vector3 ClosestPointOnLine(Vector3 from, Vector3 to, Vector3 position)
        {
            var direction = position - from;
            var directionNormalized = (to - from).normalized;
     
            var distance = Vector3.Distance(from, to);
            var dotProduct = Vector3.Dot(directionNormalized, direction);
     
            if (dotProduct <= 0) return from;
            if (dotProduct >= distance) return to;
     
            var travelDistanceOnLine = directionNormalized * dotProduct;
            var closestPoint = from + travelDistanceOnLine;
     
            return closestPoint;
        }
        
        /// <summary>
        /// Rotates the point.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="x">X angle in degrees.</param>
        /// <param name="y">Y angle in degrees.</param>
        /// <param name="z">Z angle in degrees.</param>
        /// <returns>Rotated point.</returns>
        public static Vector3 Rotate(this Vector3 point, float x = 0, float y = 0, float z = 0)
        {
            return Quaternion.Euler(x, y, z) * point;
        }
        
        /// <summary>
        /// Rotates the point around a pivot.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="pivot">Pivot.</param>
        /// <param name="x">X angle in degrees.</param>
        /// <param name="y">Y angle in degrees.</param>
        /// <param name="z">Z angle in degrees.</param>
        /// <returns>Rotated point.</returns>
        public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, float x = 0, float y = 0, float z = 0)
        {
            return (point - pivot).Rotate(x, y, z) + pivot;
        }
    }
}