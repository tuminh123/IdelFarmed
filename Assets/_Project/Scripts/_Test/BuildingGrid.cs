using System.Collections.Generic;
using UnityEngine;

public class BuildingGridCell
{
    public Building Building { get; private set; }
    public bool IsEmpty => Building == null;

    public void SetBuilding(Building building)
    {
        Building = building;
    }
}

public class BuildingGrid
{
    private readonly BuildingGridCell[,] cells;

    public int Width { get; }
    public int Height { get; }

    public BuildingGrid(int width, int height)
    {
        Width = Mathf.Max(1, width);
        Height = Mathf.Max(1, height);
        cells = new BuildingGridCell[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                cells[x, y] = new BuildingGridCell();
            }
        }
    }

    public bool CanBuild(IReadOnlyList<Vector2Int> positions)
    {
        foreach (Vector2Int position in positions)
        {
            if (!IsInside(position)) return false;
            if (!cells[position.x, position.y].IsEmpty) return false;
        }

        return true;
    }

    public void SetBuilding(Building building, IReadOnlyList<Vector2Int> positions)
    {
        foreach (Vector2Int position in positions)
        {
            if (!IsInside(position)) continue;
            cells[position.x, position.y].SetBuilding(building);
        }
    }

    public bool IsInside(Vector2Int position)
    {
        return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
    }

    public bool IsEmpty(Vector2Int position)
    {
        return IsInside(position) && cells[position.x, position.y].IsEmpty;
    }
}
