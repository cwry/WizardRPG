using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Pathfinding{

    class PathNode : IEquatable<PathNode>{
        public PathNode(PathNode parent, Vec2i position) {
            Parent = parent;
            Position = position;
        }

        public PathNode Parent { get; set; }
        public Vec2i Position { get; set; }
        public float TCost { get; set; }
        public float GCost { get; set; }
        public float HCost { get; set; }
        public float FCost { get { return GCost + HCost; } }

        public override bool Equals(object obj) {
            return Position.Equals(((PathNode)obj).Position);
        }

        public bool Equals(PathNode other) {
            return Position.Equals(other.Position);
        }

        public override int GetHashCode() {
            return Position.GetHashCode();
        }
    }

    public static Vec2i[] SearchPath(Vec2i start, Vec2i goal, Func<Vec2i, float> getTileCost) {
        var openList = new Dictionary<PathNode, PathNode>();
        var rootNode = new PathNode(null, start);
        openList.Add(rootNode, rootNode);
        var closedList = new HashSet<PathNode>();

        while (true) {
            if (openList.Count == 0) return null;

            var currentNode = openList
                .Select(kv => kv.Value)
                .Aggregate((curCheapest, node) => curCheapest == null || node.FCost < curCheapest.FCost ? node : curCheapest);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(var v in new Vec2i[] {
                new Vec2i(currentNode.Position.x + 1, currentNode.Position.y),
                new Vec2i(currentNode.Position.x - 1, currentNode.Position.y),
                new Vec2i(currentNode.Position.x, currentNode.Position.y + 1),
                new Vec2i(currentNode.Position.x, currentNode.Position.y - 1)
            }) {
                var currentTCost = getTileCost(v);
                if (currentTCost < 0) continue;
                var node = new PathNode(currentNode, v);

                if (node.Position == goal) {
                    var path = new List<Vec2i>();
                    while (node != null) {
                        path.Insert(0, node.Position);
                        node = node.Parent;
                    }
                    return path.ToArray();
                }

                if (closedList.Contains(node)) continue;

                if (!openList.ContainsKey(node)) {
                    if(shouldNodeGetDiscounted(node, node.Parent)){
                        currentTCost *= 0.99f;
                    }

                    node.TCost = currentTCost;
                    node.GCost = currentNode.GCost + node.TCost;
                    node.HCost = Math.Abs(goal.x - node.Position.x) + Math.Abs(goal.y - node.Position.y);
                    openList.Add(node, node);
                } else {
                    node = openList[node];
                    if(shouldNodeGetDiscounted(node, currentNode)) {
                        currentTCost *= 0.99f;
                    }

                    var newGCost = currentNode.GCost + currentTCost;

                    if (node.GCost > newGCost) {
                        node.Parent = currentNode;
                        node.GCost = newGCost;
                        openList[node] = node;
                    }
                }
            }
        }
    }

    static bool shouldNodeGetDiscounted(PathNode node, PathNode parent) {
        return parent.Parent != null &&
               (parent.Position.x - node.Position.x != parent.Parent.Position.x - parent.Position.x ||
               parent.Position.y - node.Position.y != parent.Parent.Position.y - parent.Position.y);
    }
}
