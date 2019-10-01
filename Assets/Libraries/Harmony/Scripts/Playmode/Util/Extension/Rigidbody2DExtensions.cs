﻿using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Rigidbody2D.
    /// </summary>
    public static class Rigidbody2DExtensions
    {
        /// <summary>
        /// Translate the Rigidbody2D. Take collisions into consideration.
        /// </summary>
        public static void Translate(this Rigidbody2D rigidbody2D, Vector2 translation)
        {
            rigidbody2D.MovePosition(rigidbody2D.position + translation.Rotate(rigidbody2D.rotation));
        }

        /// <summary>
        /// Rotates the Rigidbody2D. Take collisions into consideration.
        /// </summary>
        public static void Rotate(this Rigidbody2D rigidbody2D, float angle)
        {
            rigidbody2D.MoveRotation(rigidbody2D.rotation + angle);
        }
    }
}