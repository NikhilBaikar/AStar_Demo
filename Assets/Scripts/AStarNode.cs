using UnityEngine;

public class AStarNode
{
    public Vector2Int Pos;
    public bool Walkable;

    public int G; // cost from start
    public int H; // heuristic to end
    public int F => G + H;

    public AStarNode Parent;

    public AStarNode(Vector2Int pos, bool walkable)
    {
        Pos = pos;
        Walkable = walkable;
    }
}
