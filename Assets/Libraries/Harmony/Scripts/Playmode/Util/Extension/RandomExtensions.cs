using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Randoms.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Randomize position in a square delimited by two points.
        /// </summary>
        /// <param name="topLeft">Bottom left.</param>
        /// <param name="bottomRight">Top right.</param>
        /// <returns>Random position.</returns>
        public static Vector2 GetRandomPosition(Vector2 topLeft, Vector2 bottomRight)
        {
            return GetRandomPosition(bottomRight.x, topLeft.x, topLeft.y, bottomRight.y);
        }

        /// <summary>
        /// Randomize position in a square delimited by min and max values for X and Y.
        /// </summary>
        /// <param name="minX">Min X value (inclusive).</param>
        /// <param name="maxX">Max X value (inclusive).</param>
        /// <param name="minY">Min Y value (inclusive).</param>
        /// <param name="maxY">Max Y value (inclusive).</param>
        /// <returns>Random position.</returns>
        public static Vector2 GetRandomPosition(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }

        /// <summary>
        /// Randomize a position on a rectangle edges.
        /// </summary>
        /// <param name="center">Rectangle center.</param>
        /// <param name="height">Rectangle height.</param>
        /// <param name="width">Rectangle width.</param>
        /// <returns>Random position.</returns>
        public static Vector2 GetRandomPositionOnRectangleEdge(Vector2 center, float height, float width)
        {
            if (height < 0)
                throw new ArgumentException("Rectangle Height cannot be negative.");
            if (width < 0)
                throw new ArgumentException("Rectangle Width cannot be negative.");

            /*
             * Imagine a rectangle, like this :
             * 
             *    |---------------1---------------|
             *    |                               |
             *    |                               |
             *    |                               |
             *    4                               3
             *    |                               |
             *    |                               |
             *    |                               |
             *    |---------------2---------------|
             *    
             * We can "unfold" it in a line, like this :
             * 
             * |--------1--------|--------2--------|--------3--------|--------4--------|
             * 
             * By picking a random position on that line, we pick a random position on the rectangle edges.
             */

            var linePart = Random.Range(0, 5); //0-5 exclusive, so 0-4 inclusive 

            var randomPosition = Random.Range(-1f, 1f); //For float, random is inclusive
            var x = linePart <= 2 ? randomPosition : linePart == 3 ? 1 : -1;
            var y = linePart >= 3 ? randomPosition : linePart == 1 ? 1 : -1;

            return new Vector2(x * (width / 2), y * (height / 2)) + center;
        }

        /// <summary>
        /// Randomize a direction.
        /// </summary>
        /// <returns>Random direction. This vector is normalized.</returns>
        public static Vector2 GetRandomDirection()
        {
            return GetRandomPosition(-1, 1, -1, 1).normalized;
        }

        /// <summary>
        /// Flip a coin. Returns 1 or -1.
        /// </summary>
        /// <returns>1 or -1</returns>
        public static int GetOneOrMinusOneAtRandom()
        {
            return Random.value > 0.5f ? 1 : -1;
        }
    }
}