using System.Collections.Generic;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Node in a Graph.
    /// </summary>
    public class Node
    {
        private const int DEFAULT_NEIGHBOURS_COUNT = 4;

        private readonly List<Node> neighbours;

        /// <summary>
        /// Position of the node (in 2D).
        /// </summary>
        public Vector2 Position { get; }


        /// <summary>
        /// Position of the node (in 3D).
        /// </summary>
        public Vector3 Position3D { get; }

        /// <summary>
        /// Neighbours of this node. 
        /// </summary>
        public IReadOnlyList<Node> Neighbours => neighbours;

        private Node(Vector2 position, Vector3 position3D)
        {
            Position = position;
            Position3D = position3D;
            neighbours = new List<Node>(DEFAULT_NEIGHBOURS_COUNT);
        }

        /// <summary>
        /// Add neighbour.
        /// </summary>
        /// <param name="neighbour">Neighbour.</param>
        public void AddNeighbour(Node neighbour)
        {
            if (!neighbours.Contains(neighbour)) neighbours.Add(neighbour);
        }

        /// <summary>
        /// Remove neighbour.
        /// </summary>
        /// <param name="neighbour">Neighbour.</param>
        public void RemoveNeighbour(Node neighbour)
        {
            if (neighbours.Contains(neighbour)) neighbours.Remove(neighbour);
        }

        public override string ToString()
        {
            return Position.ToString();
        }

        /// <summary>
        /// Creates a new Node on the (X,Y) 2D plane. 
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>Created node.</returns>
        public static Node NewXY(Vector3 position)
        {
            return new Node(new Vector2(position.x, position.y), position);
        }

        /// <summary>
        /// Creates a new Node on the (X,Z) 2D plane. 
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>Created node.</returns>
        public static Node NewXZ(Vector3 position)
        {
            return new Node(new Vector2(position.x, position.z), position);
        }
    }
}