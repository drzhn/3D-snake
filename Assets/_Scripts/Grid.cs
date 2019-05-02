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

    // Реализация A*, представленная здесь: https://en.wikibooks.org/wiki/Artificial_Intelligence/Search/Heuristic_search/Astar_Search
    public void RequestPath(Vector3Int startCoord, Vector3Int goalCoord, ref List<Node> path)
    {
        if (path.Count > 2) // проверка необходимости перестраивать путь
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
                int candidateGScore = x.g + 1; // расстояние между соседями всегда 1
                bool isCandidateBetter = false;
                if (!openset.Contains(y))
                {
                    openset.Add(y);
                    isCandidateBetter = true;
                }
                else
                {
                    if (candidateGScore < y.g)
                    {
                        isCandidateBetter = true;
                    }
                    else
                    {
                        isCandidateBetter = false;
                    }
                }

                if (isCandidateBetter)
                {
                    y.cameFrom = x;
                    y.g = candidateGScore;
                    y.h = ManhattanDistance(y.coords, goalCoord);
                }
            }
        }

        path = null;
    }

    private HashSet<Node> NeighborOf(Node node, bool full = false)
    {
        // Так как мы изначально не строим весь граф, сосдеями будут считаться те клетки, координаты которых не содержатся в списке координат змеек
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

    /// <summary>
    /// Функция оценки расстояния между клетками - это манхэттенское расстояние
    /// </summary>
    private static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
    }

    /// <summary>
    /// Получение новой координаты для яблока
    /// </summary>
    public Vector3Int RequestNewAppleCoord()
    {
        // координата должна находиться на свободной клетке, поэтому мы возьмем случайную и будем перебирать всех соседей
        // а затем и их соседей, пока не найдем свободную

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