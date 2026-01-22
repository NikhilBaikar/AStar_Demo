using UnityEngine;

public class AStarGrid
{
    public int Size;
    public AStarNode[,] Nodes;

    public AStarGrid(int size)
    {
        Size = size;
        Nodes = new AStarNode[size, size];

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                Nodes[x, y] = new AStarNode(new Vector2Int(x, y), true);
    }

    public bool InBounds(Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < Size && p.y < Size;
    }
}
