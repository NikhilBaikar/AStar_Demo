using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AStarGridView : MonoBehaviour
{
    [SerializeField] private TileClick tilePrefab;
    [SerializeField] private float spacing = 1.1f;

    private TileClick[,] tiles;
    private int size;

    public Action<Vector2Int, Vector2Int> OnTileClicked;

    public Camera camera;

    private Vector2Int start;
    private Vector2Int end;

    AStarGrid grid;

    bool isStartSet = false;
    public void DrawGrid(AStarGrid g, List<Vector2Int> path)
    {
        ClearGrid();

        size = g.Size;
        grid = g;
        tiles = new TileClick[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                TileClick tile = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = new Vector3(x * spacing, 0f, y * spacing);
                tiles[x, y] = tile;
                tile.name = "Tile-" +x+y;

                tile.Init(pos, this);


                tile.r.material.color = g.Nodes[x, y].Walkable
                    ? Color.white
                    : Color.black;
            }
        }

        GetCenterPoint();
        if (path != null)
        {
            foreach (var p in path)
            {
                if (p != start && p != end)
                {
                    tiles[p.x, p.y].r.material.color = Color.yellow;
                }
            }
            tiles[end.x, end.y].r.material.color = Color.red;
            tiles[start.x, start.y].r.material.color = Color.green;
        }
        else
        {
            isStartSet = false;
        }
    }

    public void SetStartEndPos(Vector2Int pos)
    {
        if (!tiles[pos.x, pos.y])
            return;

        if (!grid.Nodes[pos.x, pos.y].Walkable)
            return;

        if (isStartSet)
        {
            if (pos == start)
            {
                start = pos;
                end = pos;
            }
            else
            {
                end = pos;
            }
                OnTileClicked?.Invoke(start, end);
        }
        else
        {
            isStartSet = true;
            start = pos;
            tiles[start.x, start.y].r.material.color = Color.green;
        }
    }


    private void ClearGrid()
    {
        if (tiles == null) return;

        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        tiles = null;
    }

    public void GetCenterPoint()
    {
        if(camera == null)
            camera = Camera.main;

        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));

        float t = (0 - ray.origin.y) / ray.direction.y;
        Vector3 worldcenter = ray.origin + ray.direction * t;

        float gridsize = size * spacing;
        float halfGrid = (gridsize - 1) * 0.5f;

        Vector3 pos = transform.position;
        pos.x = worldcenter.x - halfGrid;
        pos.z = worldcenter.z - halfGrid;
        transform.position = pos;
    }
}
