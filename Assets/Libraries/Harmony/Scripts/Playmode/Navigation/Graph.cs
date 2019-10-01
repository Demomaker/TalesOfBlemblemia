using System;
using System.Collections.Generic;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Node graph. 
    /// </summary>
    public class Graph
    {
        private readonly Region root;
        private readonly List<Node> nodes; //Cache of all nodes. Faster to access randomly for example.
        private readonly List<Region> regions; //Cache of all regions. Faster to access randomly for example.

        /// <summary>
        /// List of all nodes.
        /// </summary>
        public IReadOnlyList<Node> Nodes => nodes;

        /// <summary>
        /// List of all regions and sub-regions.
        /// </summary>
        public IReadOnlyList<Region> Regions => regions;

        /// <summary>
        /// Top center position.
        /// </summary>
        public Vector2 TopCenter => root.TopCenter;

        /// <summary>
        /// Bottom center position.
        /// </summary>
        public Vector2 BottomCenter => root.BottomCenter;

        /// <summary>
        /// Left center position.
        /// </summary>
        public Vector2 LeftCenter => root.LeftCenter;

        /// <summary>
        /// Center center position.
        /// </summary>
        public Vector2 RightCenter => root.RightCenter;

        /// <summary>
        /// Top left position.
        /// </summary>
        public Vector2 TopLeft => root.TopLeft;

        /// <summary>
        /// Top right position.
        /// </summary>
        public Vector2 TopRight => root.TopRight;

        /// <summary>
        /// Bottom left position.
        /// </summary>
        public Vector2 BottomLeft => root.BottomLeft;

        /// <summary>
        /// Bottom right position.
        /// </summary>
        public Vector2 BottomRight => root.BottomRight;

        /// <summary>
        /// Center position.
        /// </summary>
        public Vector2 Center => root.Center;

        /// <summary>
        /// Size.
        /// </summary>
        public Vector2 Size => root.Size;

        private bool isInTransaction = false;

        private Graph(Vector2 topLeft, Vector2 bottomRight)
        {
            root = new Region(topLeft, bottomRight);
            nodes = new List<Node>(0);
            regions = new List<Region>(0);
        }

        /// <summary>
        /// Find the closest Node to a specified point in this graph.
        /// </summary>
        /// <param name="position">Point to find.</param>
        /// <param name="maxDistance">Max distance to the point.</param>
        /// <returns>Node if found. Null otherwise.</returns>
        public Node Find(Vector3 position, float maxDistance = float.MaxValue)
        {
            return root.Find(new Vector2(position.x, position.z), maxDistance);
        }

        /// <summary>
        /// Begin a transaction on this graph. This is mandatory to add nodes to this graph for performance purposes.
        /// </summary>
        public void BeginTransaction()
        {
            isInTransaction = true;
        }

        /// <summary>
        /// Add a node to this graph. You must have an opened transaction to do this.
        /// </summary>
        /// <param name="node">Node.</param>
        public void Add(Node node)
        {
            if (!isInTransaction) throw new Exception("You must open a transaction to edit a Graph.");

            root.Add(node);
        }

        /// <summary>
        /// Ends the transaction on this graph. A call to this is quite slow.
        /// </summary>
        public void EndTransaction()
        {
            nodes.Clear();
            regions.Clear();

            nodes.AddRange(root.Nodes);
            regions.Add(root);
            regions.AddRange(root.Subregions);

            isInTransaction = false;
        }


        /// <summary>
        /// Create a new Graph on the (X,Y) 2D plane.
        /// </summary>
        /// <param name="topLeft">Top left position. Z coordinate is ignored.</param>
        /// <param name="bottomRight">Bottom right position. Z coordinate is ignored.</param>
        /// <returns>Created graph.</returns>
        public static Graph NewXY(Vector3 topLeft, Vector3 bottomRight)
        {
            return new Graph(new Vector2(topLeft.x, topLeft.y), new Vector2(bottomRight.x, bottomRight.y));
        }

        /// <summary>
        /// Create a new Graph on the (X,Z) 2D plane.
        /// </summary>
        /// <param name="topLeft">Top left position. Y coordinate is ignored.</param>
        /// <param name="bottomRight">Bottom right position. Y coordinate is ignored.</param>
        /// <returns>Created graph.</returns>
        public static Graph NewXZ(Vector3 topLeft, Vector3 bottomRight)
        {
            return new Graph(new Vector2(topLeft.x, topLeft.z), new Vector2(bottomRight.x, bottomRight.z));
        }
    }
}