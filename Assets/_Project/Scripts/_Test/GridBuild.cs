using System.Collections.Generic;
using UnityEngine;

public class GridBuild : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    [Header("Cell Visual")]
    [SerializeField] private SpriteRenderer cellPrefab;
    [SerializeField] private Color emptyColor = new(1f, 1f, 1f, 0.08f);
    [SerializeField] private Color occupiedColor = new(0.25f, 0.25f, 0.25f, 0.35f);
    [SerializeField] private Color positivePreviewColor = new(0f, 1f, 0.2f, 0.35f);
    [SerializeField] private Color negativePreviewColor = new(1f, 0.1f, 0f, 0.35f);
    [SerializeField] private float visualCellScale = 0.92f;
    [SerializeField] private int visualSortingOrder = 40;

    [Header("Debug")]
    [SerializeField] private bool drawGridGizmos = true;

    private BuildingGrid grid;
    private SpriteRenderer[,] cellVisuals;
    private readonly List<Vector2Int> previewCells = new();
    private Sprite defaultCellSprite;

    private void OnValidate()
    {
        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);
        visualCellScale = Mathf.Max(0.01f, visualCellScale);
    }

    private void Awake()
    {
        grid = new BuildingGrid(width, height);
        width = grid.Width;
        height = grid.Height;
        SpawnCellVisuals();
    }

    public bool CanBuild(List<Vector2Int> buildingPositions)
    {
        return grid.CanBuild(buildingPositions);
    }

    public void SetBuilding(Building building, List<Vector2Int> buildingPositions)
    {
        grid.SetBuilding(building, buildingPositions);

        foreach (Vector2Int position in buildingPositions)
        {
            if (!grid.IsInside(position)) continue;
            SetCellColor(position, occupiedColor);
        }
    }

    public void ShowPreview(List<Vector2Int> cells, bool canBuild)
    {
        ClearPreview();

        Color previewColor = canBuild ? positivePreviewColor : negativePreviewColor;

        foreach (Vector2Int cell in cells)
        {
            if (!grid.IsInside(cell)) continue;

            SetCellColor(cell, previewColor);
            previewCells.Add(cell);
        }
    }

    public void ClearPreview()
    {
        foreach (Vector2Int cell in previewCells)
        {
            if (!grid.IsInside(cell)) continue;
            SetCellColor(cell, grid.IsEmpty(cell) ? emptyColor : occupiedColor);
        }

        previewCells.Clear();
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition-transform.position).x / BuildingSystem.CellSize);
        int y = Mathf.FloorToInt((worldPosition - transform.position).y / BuildingSystem.CellSize);
        return new Vector2Int(x,y);
    }

    public Vector3 GetCellCenterWorld(Vector2Int cell)
    {
        return transform.position + new Vector3(
            cell.x * BuildingSystem.CellSize + BuildingSystem.CellSize / 2f,
            cell.y * BuildingSystem.CellSize + BuildingSystem.CellSize / 2f,
            0f
        );
    }

    public Vector3 GetCellsCenterWorld(List<Vector2Int> cells)
    {
        int minX = cells[0].x;
        int maxX = cells[0].x;
        int minY = cells[0].y;
        int maxY = cells[0].y;

        foreach (Vector2Int cell in cells)
        {
            minX = Mathf.Min(minX, cell.x);
            maxX = Mathf.Max(maxX, cell.x);
            minY = Mathf.Min(minY, cell.y);
            maxY = Mathf.Max(maxY, cell.y);
        }

        float centerX = (minX + maxX) / 2f * BuildingSystem.CellSize + BuildingSystem.CellSize / 2f;
        float centerY = (minY + maxY) / 2f * BuildingSystem.CellSize + BuildingSystem.CellSize / 2f;
        return transform.position + new Vector3(centerX, centerY, 0f);
    }

    private void SpawnCellVisuals()
    {
        cellVisuals = new SpriteRenderer[grid.Width, grid.Height];

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Vector2Int position = new(x, y);
                SpriteRenderer cellVisual = CreateCellVisual(position);
                cellVisual.color = emptyColor;
                cellVisuals[x, y] = cellVisual;
            }
        }
    }

    private void SetCellColor(Vector2Int cell, Color color)
    {
        SpriteRenderer cellVisual = cellVisuals[cell.x, cell.y];
        if (cellVisual == null) return;
        cellVisual.color = color;
    }

    private SpriteRenderer CreateCellVisual(Vector2Int cell)
    {
        SpriteRenderer renderer = cellPrefab != null
            ? Instantiate(cellPrefab, transform)
            : CreateDefaultCellVisual();

        renderer.transform.position = GetCellCenterWorld(cell);
        renderer.transform.localScale = Vector3.one * BuildingSystem.CellSize * visualCellScale;
        renderer.sortingOrder = visualSortingOrder;
        return renderer;
    }

    private SpriteRenderer CreateDefaultCellVisual()
    {
        GameObject cellObject = new("Grid Cell");
        cellObject.transform.SetParent(transform);

        SpriteRenderer renderer = cellObject.AddComponent<SpriteRenderer>();
        renderer.sprite = GetDefaultCellSprite();
        return renderer;
    }

    private Sprite GetDefaultCellSprite()
    {
        if (defaultCellSprite != null) return defaultCellSprite;

        Texture2D texture = new(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        defaultCellSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return defaultCellSprite;
    }

    private void OnDrawGizmos()
    {
        if (!drawGridGizmos) return;

        Gizmos.color = Color.yellow;
        if(BuildingSystem.CellSize <= 0 || width <= 0 || height <= 0) return;
        Vector3 origin = transform.position;
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = origin + new Vector3(0, y * BuildingSystem.CellSize ,0.01f);
            Vector3 end   = origin + new Vector3(width*BuildingSystem.CellSize, y * BuildingSystem.CellSize, 0.01f);
            Gizmos.DrawLine(start,end);
        }

        for (int x = 0; x <= width; x++)
        {
            Vector3 start = origin + new Vector3(x * BuildingSystem.CellSize, 0, 0.01f);
            Vector3 end = origin + new Vector3(x * BuildingSystem.CellSize, height * BuildingSystem.CellSize, 0.01f);
            Gizmos.DrawLine(start, end);
        }
    }
}
