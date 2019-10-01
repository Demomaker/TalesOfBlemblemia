using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Harmony
{
    /// <summary>
    /// Extension methods for Gizmos.
    /// </summary>
    public static class GizmosExtensions
    {
        private const int FONT_SIZE = 20;
        private const string POINT_IMAGE_NAME = "PointNormal.png";
        private const string POINT_ACCENT_IMAGE_NAME = "PointAccent.png";
        private const string POINT_MUTED_IMAGE_NAME = "PointMuted.png";

#if UNITY_EDITOR
        /// <summary>
        /// Draws Text in the scene. Only usable in Unity Editor.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="text">Text.</param>
        public static void DrawText(Vector3 position, string text)
        {
            DrawText(position, text, Color.white);
        }

        /// <summary>
        /// Draws Text in the scene. Only usable in Unity Editor.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="text">Text.</param>
        /// <param name="color">Color.</param>
        public static void DrawText(Vector3 position, string text, Color color)
        {
            var style = new GUIStyle
            {
                fontSize = FONT_SIZE,
                normal =
                {
                    textColor = color
                }
            };

            Handles.Label(position, text, style);
        }
#endif

        /// <summary>
        /// Draws blue point un the scene.
        /// </summary>
        /// <param name="position">Position.</param>
        public static void DrawPoint(Vector3 position)
        {
            Gizmos.DrawIcon(position, POINT_IMAGE_NAME);
        }

        /// <summary>
        /// Draws red point un the scene.
        /// </summary>
        /// <param name="position">Position.</param>
        public static void DrawPointAccent(Vector3 position)
        {
            Gizmos.DrawIcon(position, POINT_ACCENT_IMAGE_NAME);
        }

        /// <summary>
        /// Draws light grey point un the scene.
        /// </summary>
        /// <param name="position">Position.</param>
        public static void DrawPointMuted(Vector3 position)
        {
            Gizmos.DrawIcon(position, POINT_MUTED_IMAGE_NAME);
        }

        /// <summary>
        /// Draw line in the scene.
        /// </summary>
        /// <param name="from">Start position.</param>
        /// <param name="to">End position.</param>
        public static void DrawLine(Vector3 from, Vector3 to)
        {
            DrawLine(from, to, Color.white);
        }

        /// <summary>
        /// Draw line in the scene.
        /// </summary>
        /// <param name="from">Start position.</param>
        /// <param name="to">End position.</param>
        /// <param name="color">Color.</param>
        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            var colorBackup = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(from, to);
            Gizmos.color = colorBackup;
        }

        /// <summary>
        /// Draw sphere in the scene.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        public static void DrawWireSphere(Vector3 center, float radius)
        {
            DrawWireSphere(center, radius, Color.white);
        }

        /// <summary>
        /// Draw sphere in the scene.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            var colorBackup = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
            Gizmos.color = colorBackup;
        }

        /// <summary>
        /// Draw box in the scene.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="size">Size.</param>
        public static void DrawBox(Vector3 center, Vector3 size)
        {
            DrawBox(center, size, Quaternion.identity, Color.white);
        }

        /// <summary>
        /// Draw box in the scene.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="size">Size.</param>
        /// <param name="color">Color.</param>
        public static void DrawBox(Vector3 center, Vector3 size, Color color)
        {
            DrawBox(center, size, Quaternion.identity, color);
        }

        /// <summary>
        /// Draw box in the scene.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="size">Size.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="color">Color.</param>
        public static void DrawBox(Vector3 center, Vector3 size, Quaternion rotation, Color color)
        {
            var colorBackup = Gizmos.color;
            Gizmos.color = color;

            var point1 = size / 2;
            var point2 = Vector3.Reflect(point1, Vector3.right);
            var point3 = Vector3.Reflect(point2, Vector3.up);
            var point4 = Vector3.Reflect(point3, Vector3.right);
            point1 = rotation * point1 + center;
            point2 = rotation * point2 + center;
            point3 = rotation * point3 + center;
            point4 = rotation * point4 + center;

            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point2, point3);
            Gizmos.DrawLine(point3, point4);
            Gizmos.DrawLine(point4, point1);

            Gizmos.color = colorBackup;
        }

        /// <summary>
        /// Draw arrow in the scene.
        /// </summary>
        /// <param name="from">Start position.</param>
        /// <param name="to">End position. This is where the arrow points.</param>
        public static void DrawArrow(Vector3 from, Vector3 to)
        {
            DrawArrow(from, to, Color.white);
        }

        /// <summary>
        /// Draw arrow in the scene.
        /// </summary>
        /// <param name="from">Start position.</param>
        /// <param name="to">End position. This is where the arrow points.</param>
        /// <param name="color">Color.</param>
        public static void DrawArrow(Vector3 from, Vector3 to, Color color)
        {
            var colorBackup = Gizmos.color;
            Gizmos.color = color;

            var direction = to - from;
            var up = Quaternion.LookRotation(direction) * Quaternion.Euler(180 + 30, 0, 0) * Vector3.forward;
            var down = Quaternion.LookRotation(direction) * Quaternion.Euler(180 - 30, 0, 0) * Vector3.forward;
            var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 30, 0) * Vector3.forward;
            var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 30, 0) * Vector3.forward;

            Gizmos.DrawLine(from, to);
            Gizmos.DrawLine(to, to + up);
            Gizmos.DrawLine(to, to + down);
            Gizmos.DrawLine(to, to + left);
            Gizmos.DrawLine(to, to + right);
            Gizmos.color = colorBackup;
        }

        /// <summary>
        /// Draw a list of nodes in the scene with arrows.
        /// </summary>
        /// <param name="path">List of nodes.</param>
        public static void DrawPath(IReadOnlyList<Node> path)
        {
            for (var i = 1; i < path.Count; i++)
            {
                var previousPosition = path[i - 1].Position3D;
                var currentPosition = path[i].Position3D;
                DrawArrow(previousPosition, currentPosition, Color.cyan);
            }
        }
    }
}