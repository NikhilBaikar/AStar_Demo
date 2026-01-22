using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder
{
    private static readonly Vector2Int[] DIRS_4 =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private static readonly Vector2Int[] DIRS_8 =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    public static List<Vector2Int> FindPath(
        AStarGrid grid,
        Vector2Int start,
        Vector2Int end,
        bool allowDiagonal)
    {
        ResetNodes(grid);

        var open = new List<AStarNode>();
        var closed = new HashSet<AStarNode>();

        AStarNode startNode = grid.Nodes[start.x, start.y];
        AStarNode endNode = grid.Nodes[end.x, end.y];

        startNode.G = 0;
        startNode.H = Heuristic(start, end, allowDiagonal);
        startNode.Parent = null;

        open.Add(startNode);

        var directions = allowDiagonal ? DIRS_8 : DIRS_4;

        while (open.Count > 0)
        {
            // Pick node with lowest F
            open.Sort((a, b) => a.F.CompareTo(b.F));
            AStarNode current = open[0];

            if (current == endNode)
                return ReconstructPath(current);

            open.Remove(current);
            closed.Add(current);

            foreach (var dir in directions)
            {
                Vector2Int nextPos = current.Pos + dir;

                if (!grid.InBounds(nextPos))
                    continue;

                AStarNode neighbour = grid.Nodes[nextPos.x, nextPos.y];

                if (!neighbour.Walkable || closed.Contains(neighbour))
                    continue;

                // Prevent diagonal corner cutting
                if (allowDiagonal && dir.x != 0 && dir.y != 0)
                {
                    Vector2Int n1 = new Vector2Int(current.Pos.x + dir.x, current.Pos.y);
                    Vector2Int n2 = new Vector2Int(current.Pos.x, current.Pos.y + dir.y);

                    if (!grid.Nodes[n1.x, n1.y].Walkable ||
                        !grid.Nodes[n2.x, n2.y].Walkable)
                        continue;
                }

                int moveCost = (dir.x != 0 && dir.y != 0) ? 14 : 10;
                int tentativeG = current.G + moveCost;

                if (!open.Contains(neighbour) || tentativeG < neighbour.G)
                {
                    neighbour.G = tentativeG;
                    neighbour.H = Heuristic(neighbour.Pos, end, allowDiagonal);
                    neighbour.Parent = current;

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }

        return null; // no path
    }

    private static int Heuristic(Vector2Int a, Vector2Int b, bool allowDiagonal)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        if (!allowDiagonal)
        {
            // Manhattan distance
            return 10 * (dx + dy);
        }
        else
        {
            // Octile distance (correct for diagonals)
            return 10 * (dx + dy) + (14 - 20) * Mathf.Min(dx, dy);
        }
    }

    private static List<Vector2Int> ReconstructPath(AStarNode node)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (node != null)
        {
            path.Add(node.Pos);
            node = node.Parent;
        }

        path.Reverse();
        return path;
    }

    private static void ResetNodes(AStarGrid grid)
    {
        for (int x = 0; x < grid.Size; x++)
        {
            for (int y = 0; y < grid.Size; y++)
            {
                AStarNode n = grid.Nodes[x, y];
                n.G = int.MaxValue;
                n.H = 0;
                n.Parent = null;
            }
        }
    }
}
