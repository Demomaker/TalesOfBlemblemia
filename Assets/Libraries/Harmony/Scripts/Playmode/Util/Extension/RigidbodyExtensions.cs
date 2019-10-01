﻿using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Rigidbodies.
    /// </summary>
    public static class RigidbodyExtensions
    {
        /// <summary>
        /// Translate the Rigidbody. Take collisions into consideration.
        /// </summary>
        public static void Translate(this Rigidbody rigidbody, Vector3 translation)
        {
            rigidbody.MovePosition(rigidbody.position + rigidbody.rotation * translation);
        }

        /// <summary>
        /// Rotates the Rigidbody. Take collisions into consideration.
        /// </summary>
        public static void Rotate(this Rigidbody rigidbody, Vector3 axis, float angle)
        {
            rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(axis.normalized * angle));
        }
    }
}