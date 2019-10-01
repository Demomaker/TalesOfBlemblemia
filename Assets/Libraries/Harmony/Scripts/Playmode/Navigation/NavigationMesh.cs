using System.Collections.Generic;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Navigation Mesh.
    ///
    /// This does not autogenerates the mesh. You must give it a graph to work with.
    /// </summary>
    [Findable(R.S.Tag.NavigationMesh)]
    public class NavigationMesh : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Debug")] [SerializeField] private DebugType show = DebugType.NodesAndLinks;
#endif
        private Graph graph;

        /// <summary>
        /// Graph.
        /// </summary>
        public Graph Graph
        {
            get => graph;
            set => graph = value;
        }

        /// <summary>
        /// Nodes.
        /// </summary>
        public IReadOnlyList<Node> Nodes => graph.Nodes;

        /// <summary>
        /// Find the closest Node to a specified point in this graph.
        /// </summary>
        /// <param name="position">Point to find.</param>
        /// <param name="maxDistance">Max distance to the point.</param>
        /// <returns>Node if found. Null otherwise.</returns>
        public Node Find(Vector3 position, float maxDistance = float.MaxValue)
        {
            return graph?.Find(position, maxDistance);
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (graph != null)
            {
                switch (show)
                {
                    case DebugType.Nodes:
                        ShowNodes();
                        break;
                    case DebugType.Links:
                        ShowLinks();
                        break;
                    case DebugType.NodesAndLinks:
                        ShowNodes();
                        ShowLinks();
                        break;
                }
            }
        }

        private void ShowNodes()
        {
            foreach (var node in graph.Nodes)
            {
                GizmosExtensions.DrawPoint(node.Position3D);
            }
        }

        private void ShowLinks()
        {
            foreach (var node in graph.Nodes)
            {
                foreach (var neighbour in node.Neighbours)
                {
                    GizmosExtensions.DrawLine(node.Position3D, neighbour.Position3D, Color.cyan);
                }
            }
        }

        private enum DebugType
        {
            // ReSharper disable once UnusedMember.Local
            None,
            Nodes,
            Links,
            NodesAndLinks,
        }

#endif
    }
}