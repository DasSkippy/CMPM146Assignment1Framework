using UnityEngine;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour
{
    // Assignment 2: Implement AStar
    //
    // DO NOT CHANGE THIS SIGNATURE (parameter types + return type)
    // AStar will be given the start node, destination node and the target position, and should return 
    // a path as a list of positions the agent has to traverse to reach its destination, as well as the
    // number of nodes that were expanded to find this path
    // The last entry of the path will be the target position, and you can also use it to calculate the heuristic
    // value of nodes you add to your search frontier; the number of expanded nodes tells us if your search was
    // efficient
    //
    // Take a look at StandaloneTests.cs for some test cases

    private class AStarEntry
    {
        public GraphNode node;
        public AStarEntry parent;
        public float cost;
        public float costplusheuristic;
        public Vector3 waypoint;

        public AStarEntry(GraphNode node, AStarEntry parent, float cost, float costplusheuristic, Vector3 waypoint)
        {
            this.node = node;
            this.parent = parent;
            this.cost = cost;
            this.costplusheuristic = costplusheuristic;
            this.waypoint = waypoint;
        }
    }
    public static (List<Vector3>, int) AStar(GraphNode start, GraphNode destination, Vector3 target)
    {
        if (start == null || destination == null)
            return (new List<Vector3>(), 0);
        if (start.GetID() == destination.GetID())
            return (new List<Vector3> { target }, 0);

        var frontier = new List<AStarEntry>();
        var entryMap = new Dictionary<int, AStarEntry>();
        var closed = new HashSet<int>();
        int expanded = 0;

        float h0 = Vector3.Distance(start.GetCenter(), target);
        var startEntry = new AStarEntry(start, null, 0f, h0, Vector3.zero);
        frontier.Add(startEntry);
        entryMap[start.GetID()] = startEntry;

        while (frontier.Count > 0)
        {
            AStarEntry current = frontier[0];
            for (int i = 1; i < frontier.Count; i++)
                if (frontier[i].costplusheuristic < current.costplusheuristic)
                    current = frontier[i];

            if (current.node.GetID() == destination.GetID())
            {
                var path = new List<Vector3>();
                while (current.parent != null)
                {
                    path.Add(current.waypoint);
                    current = current.parent;
                }
                path.Reverse();
                path.Add(target);
                return (path, expanded);
            }

            frontier.Remove(current);
            entryMap.Remove(current.node.GetID());
            closed.Add(current.node.GetID());
            expanded++;

            foreach (var neighbor in current.node.GetNeighbors())
            {
                var neighborNode = neighbor.GetNode();
                int id = neighborNode.GetID();
                if (closed.Contains(id)) 
                    continue;

                float stepCost = Vector3.Distance(current.node.GetCenter(), neighborNode.GetCenter());
                float newCost = current.cost + stepCost;
                float newheuristic = Vector3.Distance(neighborNode.GetCenter(), target);
                float newcostplush = newCost + newheuristic;
                Vector3 wp = neighbor.GetWall().midpoint;

                if (entryMap.TryGetValue(id, out var existing))
                {
                    if (newcostplush >= existing.costplusheuristic)
                        continue;
                    frontier.Remove(existing);
                    entryMap.Remove(id);
                }
                var entry = new AStarEntry(neighborNode, current, newCost, newcostplush, wp);
                frontier.Add(entry);
                entryMap[id] = entry;
            }
        }
        return (new List<Vector3>(), expanded);
    }

    public Graph graph;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus.OnTarget += PathFind;
        EventBus.OnSetGraph += SetGraph;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGraph(Graph g)
    {
        graph = g;
    }

    // entry point
    public void PathFind(Vector3 target)
    {
        if (graph == null) return;

        // find start and destination nodes in graph
        GraphNode start = null;
        GraphNode destination = null;
        foreach (var n in graph.all_nodes)
        {
            if (Util.PointInPolygon(transform.position, n.GetPolygon()))
            {
                start = n;
            }
            if (Util.PointInPolygon(target, n.GetPolygon()))
            {
                destination = n;
            }
        }
        if (destination != null)
        {
            // only find path if destination is inside graph
            EventBus.ShowTarget(target);
            (List<Vector3> path, int expanded) = PathFinder.AStar(start, destination, target);

            Debug.Log("found path of length " + path.Count + " expanded " + expanded + " nodes, out of: " + graph.all_nodes.Count);
            EventBus.SetPath(path);
        }
        

    }

    

 
}

