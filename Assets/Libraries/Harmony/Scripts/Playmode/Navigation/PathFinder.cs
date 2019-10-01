using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Harmony
{
    /// <summary>
    /// PathFinder. Uses the A* algorithm.
    /// </summary>
    [Findable(R.S.Tag.NavigationMesh)]
    public sealed class PathFinder : MonoBehaviour
    {
        private NavigationMesh navigationMesh;
        private Random random;

        private void Awake()
        {
            navigationMesh = Finder.NavigationMesh;
        }

        /// <summary>
        /// Find a path from "start" to "end".
        /// </summary>
        /// <param name="start">Start position.</param>
        /// <param name="end">End position.</param>
        /// <returns>
        /// List of nodes to walk in order to go from "start" to "end", excluding "start".
        /// Might be empty if "start" == "end".
        /// Will be null if "start" node cannot be found. 
        /// Will be null if "end" node cannot be found. 
        /// Will be null if no path was found.
        /// </returns>
        public List<Node> FindPath(Vector3 start, Vector3 end)
        {
            var startNode = navigationMesh.Find(start);
            if (startNode == null) return null;

            var endNode = navigationMesh.Find(end);
            if (endNode == null) return null;

            //A* algorithm.

            //From each node, which node it can most efficiently be reached from.
            var previous = new Dictionary<Node, Node>();
            //Known nodes not yet evaluated.
            var openedNodes = new HashSet<Node>();
            //Evaluated nodes
            var closedNodes = new HashSet<Node>();
            //Cost from start node to a specific node.
            var costToNode = new Dictionary<Node, float>();
            //Cost from start node to end node, passing by a specific node.
            var costToEnd = new Dictionary<Node, float>();

            //Cost to start node is 0.
            openedNodes.Add(startNode);
            costToNode[startNode] = 0;
            costToEnd[startNode] = Vector2.Distance(startNode.Position, endNode.Position);

            bool pathFound = false;
            while (openedNodes.Count > 0)
            {
                var currentNode = GetLeastCostToEndNode(openedNodes, costToEnd);

                if (currentNode == endNode)
                {
                    pathFound = true;
                    break;
                }

                openedNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                var costToCurrentNode = costToNode.ContainsKey(currentNode) ? costToNode[currentNode] : float.MaxValue;

                foreach (var neighbourNode in currentNode.Neighbours)
                {
                    if (!closedNodes.Contains(neighbourNode))
                    {
                        openedNodes.Add(neighbourNode); //openedNodes is "HashSet". Therefore, it can't have duplicates.

                        var newCostToNeighbour = costToCurrentNode +
                                                 Vector2.Distance(currentNode.Position, neighbourNode.Position);

                        var costToNeighbourNode = costToNode.ContainsKey(neighbourNode)
                            ? costToNode[neighbourNode]
                            : float.MaxValue;
                        if (newCostToNeighbour < costToNeighbourNode)
                        {
                            var neighbourCostToEnd = newCostToNeighbour +
                                                     Vector2.Distance(currentNode.Position, endNode.Position);

                            previous[neighbourNode] = currentNode;
                            costToNode[neighbourNode] = newCostToNeighbour;
                            costToEnd[neighbourNode] = neighbourCostToEnd;
                        }
                    }
                }
            }

            return pathFound ? GetPathFromPrevious(previous, endNode) : null;
        }


        /// <summary>
        /// Find a random path from "start". Tries to go in a random direction.
        /// </summary>
        /// <param name="start">Start position.</param>
        /// <param name="minSteps">Minimal number of steps. Please note that this might not be feasible.</param>
        /// <param name="maxSteps">Maximum number of steps. Please note that this might not be feasible.</param>
        /// <returns>
        /// List of nodes to walk in order to go from "start".
        /// Might be empty if it's impossible to go anywhere from the node (i.e a node with no neighbours).
        /// Will be null if "start" node cannot be found. 
        /// </returns>
        public List<Node> FindRandomWalk(Vector3 start, int minSteps, int maxSteps)
        {
            if (minSteps > maxSteps)
                throw new ArgumentException($"{nameof(minSteps)} must be lower or equal to {nameof(maxSteps)}");

            var startNode = navigationMesh.Find(start);
            if (startNode == null) return null;

            var path = new List<Node>();
            var pathOptions = new List<Node>();

            var nbSteps = random.Next(minSteps, maxSteps);
            var currentNode = startNode;
            var sqrDistanceFromStart = 0f;
            for (var i = 0; i < nbSteps && currentNode != null; i++)
            {
                pathOptions.AddRange(currentNode.Neighbours);

                while (pathOptions.Count > 0)
                {
                    currentNode = pathOptions.RemoveRandom(random);
                    var sqDistanceToCurrentNode = startNode.Position.SqrDistanceTo(currentNode.Position);
                    if (sqDistanceToCurrentNode > sqrDistanceFromStart)
                    {
                        path.Add(currentNode);
                        sqrDistanceFromStart = sqDistanceToCurrentNode;
                        break;
                    }

                    //No more options are available to go farther.
                    if (pathOptions.Count == 0)
                    {
                        currentNode = null;
                        break;
                    }
                }

                pathOptions.Clear();
            }

            return path;
        }

        /// <summary>
        /// Find a path from "start" to flee a specified position.
        /// </summary>
        /// <param name="start">Start position.</param>
        /// <param name="toFlee">Position to flee.</param>
        /// <returns>
        /// Next node to walk in order to flee this position from "start".
        /// Will be null if it's impossible to go flee this position (i.e there is no escape).
        /// Will be null if "start" node cannot be found. 
        /// </returns>
        public Node FindFleePath(Vector3 start, Vector3 toFlee)
        {
            var startNode = navigationMesh.Find(start);
            if (startNode == null) return null;

            var sqrDistanceFleeToStart = toFlee.SqrDistanceTo(start);
            var fleeOptions = startNode.Neighbours.ToList();
            while (fleeOptions.Count > 0)
            {
                var currentNode = fleeOptions.RemoveRandom(random);
                if (toFlee.SqrDistanceTo(currentNode.Position3D) > sqrDistanceFleeToStart)
                {
                    return currentNode;
                }
            }

            return null;
        }

        private static Node GetLeastCostToEndNode(
            IEnumerable<Node> openedNodes,
            IReadOnlyDictionary<Node, float> costToEnd
        )
        {
            var leastCost = float.MaxValue;
            Node leastCostNode = null;
            foreach (var currentNode in openedNodes)
            {
                var nodeCostToEnd = costToEnd.ContainsKey(currentNode) ? costToEnd[currentNode] : float.MaxValue;
                if (nodeCostToEnd < leastCost)
                {
                    leastCost = nodeCostToEnd;
                    leastCostNode = currentNode;
                }
            }

            return leastCostNode;
        }

        private static List<Node> GetPathFromPrevious(
            IReadOnlyDictionary<Node, Node> previous,
            Node endNode
        )
        {
            var path = new List<Node>();

            var currentNode = endNode;
            path.Add(endNode);

            while (previous.ContainsKey(currentNode))
            {
                currentNode = previous[currentNode];
                path.Insert(0, currentNode);
            }

            path.RemoveAt(0); //Remove first, because that's the start of the path, and we're already there.

            return path;
        }
    }
}