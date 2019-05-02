using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : IEquatable<Node>, IComparable<Node>
{
    public Vector3Int coords;
    public int g;
    public int h;
    public int f => g + h;
    public Node cameFrom;

    public bool Equals(Node obj)
    {
        return coords == obj?.coords;
    }

    public static bool operator ==(Node a, Node b)
    {
        if ((object) a != null && (object) b != null)
            return a.coords == b.coords;
        if ((object) a == null && (object) b == null)
            return true;
        return false;
    }

    public static bool operator !=(Node a, Node b)
    {
        if ((object) a != null && (object) b != null)
            return a.coords != b.coords;
        if ((object) a == null && (object) b == null)
            return false;
        return true;
    }

    public override string ToString()
    {
        return coords.ToString() + " " + g + " " + h;
    }

    public int CompareTo(Node other)
    {
        if (f > other.f) return 1;
        if (f < other.f) return -1;
        return 0;
    }
}


public class NodeComparer : IComparer<Node>
{
    public int Compare(Node x, Node y)
    {
        if (x.f > y.f) return 1;
        if (x.f < y.f) return -1;
        return 0;
    }
}