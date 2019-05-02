using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid
{
    private readonly List<Snake> snakes;
    private uint width;
    private uint height;
    private uint depth;

    public Grid(uint width, uint height, uint depth, List<Snake> snakes)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.snakes = snakes;
    }

    public void RequestPath(Vector3Int startCoord, Vector3Int goalCoord, ref List<Node> path)
    {
        if (path.Count > 2)
        {
            path.RemoveAt(path.Count - 1);
            if (path.TrueForAll(node => snakes.TrueForAll(snake => !snake.segments.Contains(node.coords))))
            {
                return;
            }
        }

        Node start = new Node() {coords = startCoord, g = 0, h = ManhattanDistance(startCoord, goalCoord)};
        Node goal = new Node() {coords = goalCoord};
        List<Node> closedset = new List<Node>();
        List<Node> openset = new List<Node>() {start};
        while (openset.Count > 0)
        {
            Node x = openset.Min();
            if (x == goal)
            {
                path.Clear();
                var current = x;
                while (current != null)
                {
                    path.Add(current);
                    current = current.cameFrom;
                }

                return;
            }

            openset.Remove(x);
            closedset.Add(x);
            var neighbors = NeighborOf(x, false);
            foreach (var y in neighbors)
            {
                if (closedset.Contains(y)) continue;
                int tentative_g_score = x.g + 1; //ManhattanDistance(x.coords, y.coords);
                bool tentative_is_better = false;
                if (!openset.Contains(y))
                {
                    openset.Add(y);
                    tentative_is_better = true;
                }
                else
                {
                    if (tentative_g_score < y.g)
                    {
                        tentative_is_better = true;
                    }
                    else
                    {
                        tentative_is_better = false;
                    }
                }

                if (tentative_is_better)
                {
                    y.cameFrom = x;
                    y.g = tentative_g_score;
                    y.h = ManhattanDistance(y.coords, goalCoord);
                }
            }
        }

        path = null;
    }

    public HashSet<Node> NeighborOf(Node node, bool full = false)
    {
        HashSet<Node> ret = new HashSet<Node>();

        if (node.coords.x >= 1)
        {
            Vector3Int coords = new Vector3Int(node.coords.x - 1, node.coords.y, node.coords.z);
            if (full || snakes.TrueForAll(x => !x.segments.Contains(coords)))
                ret.Add(new Node() {coords = coords});
        }

        if (node.coords.x <= width - 2)
        {
            Vector3Int coords = new Vector3Int(node.coords.x + 1, node.coords.y, node.coords.z);
            if (full || snakes.TrueForAll(x => !x.segments.Contains(coords)))
                ret.Add(new Node() {coords = coords});
        }

        if (node.coords.y >= 1)
        {
            Vector3Int coords = new Vector3Int(node.coords.x, node.coords.y - 1, node.coords.z);
            if (full || snakes.TrueForAll(x => !x.segments.Contains(coords)))
                ret.Add(new Node() {coords = coords});
        }

        if (node.coords.y <= height - 2)
        {
            Vector3Int coords = new Vector3Int(node.coords.x, node.coords.y + 1, node.coords.z);
            if (full || snakes.TrueForAll(x => !x.segments.Contains(coords)))
                ret.Add(new Node() {coords = coords});
        }

        if (node.coords.z >= 1)
        {
            Vector3Int coords = new Vector3Int(node.coords.x, node.coords.y, node.coords.z - 1);
            if (full || snakes.TrueForAll(x => !x.segments.Contains(coords)))
                ret.Add(new Node() {coords = coords});
        }

        if (node.coords.z <= depth - 2)
        {
            Vector3Int coords = new Vector3Int(node.coords.x, node.coords.y, node.coords.z + 1);
            if (full || snakes.TrueForAll(x => !x.segments.Contains(coords)))
                ret.Add(new Node() {coords = coords});
        }

        return ret;
    }

    private static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
    }

    public Vector3Int RequestNewAppleCoord()
    {
        Node randomNode = new Node()
        {
            coords = new Vector3Int(
                (int) UnityEngine.Random.Range(0, width - 1),
                (int) UnityEngine.Random.Range(0, height - 1),
                (int) UnityEngine.Random.Range(0, depth - 1))
        };
        List<Node> openset = new List<Node>() {randomNode};
        List<Node> closedset = new List<Node>();
        while (openset.Count > 0)
        {
            int count = openset.Count;
            for (int i = 0; i < count; i++)
            {
                var node = openset[i];
                if (snakes.TrueForAll(x => !x.segments.Contains(node.coords)))
                    return node.coords;
                closedset.Add(node);
                openset.Remove(node);
                openset.AddRange(NeighborOf(node, true).Where(x => !closedset.Contains(x)));
            }
        }

        throw new Exception("There's no place for apple");
    }
}