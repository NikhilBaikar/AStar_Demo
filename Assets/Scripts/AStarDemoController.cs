using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AStarDemoController : MonoBehaviour
{
    [Range(2f, 9f)]
    public int gridSize = 7;
    public AStarGridView view;

    private AStarGrid grid;
    private Vector2Int start;
    private Vector2Int end;

    public bool allowDiagonal = false;

    [Header("UI")]
    public GameObject MainScreen;
    public GameObject EditGridPanel;

    public TMP_Text lbl_gridvalue;
    public Slider slider;
    public Toggle allowdig;
    bool isEditPanelOpen = false;

    bool tempAllowDiagonal = false;
    int tempGridSize = 7;
    public void ToggAllowDiagonal(bool val)
    {
        tempAllowDiagonal = val;
    }

    void Start()
    {
        grid = new AStarGrid(gridSize);

        start = new Vector2Int(0, 0);
        end = new Vector2Int(gridSize - 1, gridSize - 1);

        slider.minValue = 2f;
        slider.maxValue = 9f;
        slider.value = gridSize;
        lbl_gridvalue.text = gridSize.ToString();

        allowdig.isOn = allowDiagonal;
        EditGridPanel.SetActive(false);

        view.OnTileClicked += CalculatePath;

        Recompute();
    }

    public void EditGrid()
    {
        EditGridPanel.SetActive(true);
        MainScreen.SetActive(false);
        isEditPanelOpen = true;
    }

    public void CloseEditPanel()
    {
        MainScreen.SetActive(true);
        EditGridPanel.SetActive(false);
        isEditPanelOpen = false;

        slider.value = gridSize;
        lbl_gridvalue.text = gridSize.ToString();
        allowdig.isOn = allowDiagonal;
    }

    public void ChangeGridSize()
    {
        tempGridSize = (int)slider.value;
        lbl_gridvalue.text = slider.value.ToString();
    }
    public void GenrateGrid()
    {
        gridSize = tempGridSize;
        allowDiagonal = tempAllowDiagonal;
        CloseEditPanel();
        Recompute();
    }

    private void Recompute()
    {
        ChangeGrid();
        view.DrawGrid(grid, null);
    }

    private void CalculatePath(Vector2Int start, Vector2Int end)
    {
        var path = AStarPathfinder.FindPath(grid, start, end, allowDiagonal);
        view.DrawGrid(grid, path);

    }
    bool PointerPressedThisFrame()
    {
        if (Pointer.current == null)
            return false;

        return Pointer.current.press.wasPressedThisFrame;
    }
    Vector2 GetPointerScreenPosition()
    {
        if (Pointer.current == null)
            return Vector2.zero;

        return Pointer.current.position.ReadValue();
    }


    public void Update()
    {
        if(!PointerPressedThisFrame())
            return;

        if (isEditPanelOpen)
            return;

        Vector2 screenPos = GetPointerScreenPosition();
        Ray ray = view.camera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
        Debug.Log("Mouse click in update " + hit.collider.name + " "+ hit.collider.GetComponent<TileClick>());
            TileClick tile = hit.collider.GetComponent<TileClick>();
            if (tile != null)
            {
                tile.OnClick();
            }
        }
    }

    public void ChangeGrid()
    {
        grid = new AStarGrid(gridSize);

        AddRandomObstacles(grid, obstaclePercent: 0.25f);

        start = new Vector2Int(0, 0);
        end = new Vector2Int(gridSize - 1, gridSize - 1);

        grid.Nodes[start.x, start.y].Walkable = true;
        grid.Nodes[end.x, end.y].Walkable = true;

        view.DrawGrid(grid, null);
    }
    private void AddRandomObstacles(AStarGrid grid, float obstaclePercent)
    {
        int totalTiles = grid.Size * grid.Size;
        int obstacleCount = Mathf.RoundToInt(totalTiles * obstaclePercent);

        for (int i = 0; i < obstacleCount; i++)
        {
            int x = Random.Range(0, grid.Size);
            int y = Random.Range(0, grid.Size);

            // Don’t block start/end area
            if ((x == 0 && y == 0) ||
                (x == grid.Size - 1 && y == grid.Size - 1))
                continue;

            grid.Nodes[x, y].Walkable = false;
        }
    }

}
