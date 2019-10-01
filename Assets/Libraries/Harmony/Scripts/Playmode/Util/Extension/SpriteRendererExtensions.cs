using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for SpriteRenderers.
    /// </summary>
    public static class SpriteRendererExtensions
    {
        /// <summary>
        /// Size of the SpriteRenderer in the World.
        /// </summary>
        public static Vector2 Size(this SpriteRenderer renderer)
        {
            var sprite = renderer.sprite;

            return new Vector2(
                sprite.texture.width / sprite.pixelsPerUnit,
                sprite.texture.height / sprite.pixelsPerUnit
            );
        }
    }
}