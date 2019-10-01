using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Cameras.
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>
        /// Checks if a point is visible in the camera.
        /// </summary>
        /// <param name="camera">Camera.</param>
        /// <param name="position">Position</param>
        public static bool IsPointInCamera(this Camera camera, Vector3 position)
        {
            var worldPosition = camera.WorldToViewportPoint(position);
            return worldPosition.x >= 0 && worldPosition.x <= 1 && worldPosition.y >= 0 && worldPosition.y <= 1;
        }

        /// <summary>
        /// Converts Viewport position to World position.
        /// </summary>
        /// <param name="camera">Camera.</param>
        /// <param name="position">Position.</param>
        public static Vector3 ViewportToWorld(this Camera camera, Vector2 position)
        {
            return camera.ViewportToWorldPoint(new Vector3(position.x, position.y, camera.nearClipPlane));
        }

        /// <summary>
        /// Left side position of the Camera in the World.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public static Vector3 LeftWorldPoint(this Camera camera)
        {
            return camera.ViewportToWorld(new Vector2(0, 0.5f));
        }

        /// <summary>
        /// Top left corner position of the Camera in the World.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public static Vector3 TopLeftWorldPoint(this Camera camera)
        {
            return camera.ViewportToWorld(new Vector2(0, 1f));
        }

        /// <summary>
        /// Bottom Left corner position of the Camera in the World.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public static Vector3 BottomLeftWorldPoint(this Camera camera)
        {
            return camera.ViewportToWorld(new Vector2(0, 0f));
        }

        /// <summary>
        /// Right side position of the Camera in the World.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public static Vector3 RightWorldPoint(this Camera camera)
        {
            return camera.ViewportToWorld(new Vector2(1, 0.5f));
        }

        /// <summary>
        /// Top right corner position of the Camera in the World.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public static Vector3 TopRightWorldPoint(this Camera camera)
        {
            return camera.ViewportToWorld(new Vector2(1, 1f));
        }

        /// <summary>
        /// Bottom right corner position of the Camera in the World.
        /// </summary>
        /// <param name="camera">Camera.</param>
        /// <returns>Position du bord gauche de la carméra.</returns>
        public static Vector3 BottomRightWorldPoint(this Camera camera)
        {
            return camera.ViewportToWorld(new Vector2(1, 0f));
        }

        /// <summary>
        /// Top side position of the Camera in the World.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public static Vector3 TopWorldPoint(this Camera camera)
        {
            return camera.ViewportToWorld(new Vector2(0.5f, 1f));
        }

        /// <summary>
        /// Bottom side position of the Camera in the World.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public static Vector3 BottomWorldPoint(this Camera camera)
        {
            return camera.ViewportToWorld(new Vector2(0.5f, 0f));
        }
    }
}