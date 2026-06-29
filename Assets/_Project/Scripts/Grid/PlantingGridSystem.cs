using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlantingCell<TPlant> where TPlant : class
{
    public Vector2Int Position { get; }
    public TPlant Plant { get; private set; }
    public bool HasPlant => Plant != null;

    public PlantingCell(Vector2Int position)
    {
        Position = position;
    }

    public bool TryPlant(TPlant plant)
    {
        if (plant == null || HasPlant)
        {
            return false;
        }

        Plant = plant;
        return true;
    }

    public bool TryClearPlant()
    {
        if (!HasPlant)
        {
            return false;
        }

        Plant = null;
        return true;
    }
}

public sealed class PlantingGridSystem<TPlant> where TPlant : class
{
    #region Fields

    private readonly Grid2D<PlantingCell<TPlant>> grid;

    #endregion

    #region Properties

    public Grid2D<PlantingCell<TPlant>> Grid => grid;

    #endregion

    #region Constructor

    public PlantingGridSystem(int width, int height, float cellSize = 1f, Vector3 origin = default)
    {
        grid = new Grid2D<PlantingCell<TPlant>>(width, height, cellSize, origin);
    }

    #endregion

    #region Area Initialization

    public bool TryCreateInitialArea(Vector2Int startPosition, Vector2Int size)
    {
        if (size.x <= 0 || size.y <= 0)
        {
            return false;
        }

        List<Vector2Int> positions = GetRectanglePositions(startPosition, size);

        if (!CanAddCells(positions, requireAdjacentArea: false))
        {
            return false;
        }

        foreach (Vector2Int position in positions)
        {
            grid.TrySet(position, new PlantingCell<TPlant>(position));
        }

        return true;
    }

    #endregion

    #region Area Query

    public bool HasCell(Vector2Int position)
    {
        return grid.TryGet(position, out PlantingCell<TPlant> cell) && cell != null;
    }

    public bool TryGetCell(Vector2Int position, out PlantingCell<TPlant> cell)
    {
        return grid.TryGet(position, out cell) && cell != null;
    }

    public IEnumerable<Vector2Int> GetAreaPositions()
    {
        foreach (Vector2Int position in grid.GetPositions())
        {
            if (HasCell(position))
            {
                yield return position;
            }
        }
    }

    public IEnumerable<PlantingCell<TPlant>> GetAreaCells()
    {
        foreach (PlantingCell<TPlant> cell in grid.GetValues())
        {
            yield return cell;
        }
    }

    #endregion

    #region Area Expansion

    public bool CanAddCell(Vector2Int position)
    {
        return CanAddCell(position, requireAdjacentArea: true);
    }

    public bool TryAddCell(Vector2Int position)
    {
        if (!CanAddCell(position))
        {
            return false;
        }

        return grid.TrySet(position, new PlantingCell<TPlant>(position));
    }

    public bool TryAddCells(IEnumerable<Vector2Int> positions)
    {
        if (!CanAddCells(positions, requireAdjacentArea: true))
        {
            return false;
        }

        foreach (Vector2Int position in positions)
        {
            grid.TrySet(position, new PlantingCell<TPlant>(position));
        }

        return true;
    }

    private bool CanAddCells(IEnumerable<Vector2Int> positions, bool requireAdjacentArea)
    {
        if (positions == null)
        {
            return false;
        }

        HashSet<Vector2Int> pendingPositions = new();

        foreach (Vector2Int position in positions)
        {
            if (!CanAddCell(position, requireAdjacentArea: false))
            {
                return false;
            }

            pendingPositions.Add(position);
        }

        if (!requireAdjacentArea)
        {
            return pendingPositions.Count > 0;
        }

        foreach (Vector2Int position in pendingPositions)
        {
            if (HasAdjacentAreaCell(position, pendingPositions))
            {
                return true;
            }
        }

        return false;
    }

    private bool CanAddCell(Vector2Int position, bool requireAdjacentArea)
    {
        if (!grid.IsInside(position) || HasCell(position))
        {
            return false;
        }

        return !requireAdjacentArea || HasAdjacentAreaCell(position);
    }

    private bool HasAdjacentAreaCell(Vector2Int position, HashSet<Vector2Int> ignoredPositions = null)
    {
        foreach (Vector2Int neighbor in grid.GetNeighborPositions(position, GridDirectionMode.FourDirections))
        {
            if (ignoredPositions != null && ignoredPositions.Contains(neighbor))
            {
                continue;
            }

            if (HasCell(neighbor))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Area Removal

    public bool CanRemoveCell(Vector2Int position)
    {
        if (!TryGetCell(position, out PlantingCell<TPlant> cell))
        {
            return false;
        }

        return !cell.HasPlant;
    }

    public bool TryRemoveCell(Vector2Int position)
    {
        if (!CanRemoveCell(position))
        {
            return false;
        }

        grid.Clear(position);
        return true;
    }

    #endregion

    #region Planting

    public bool CanPlant(Vector2Int position)
    {
        return TryGetCell(position, out PlantingCell<TPlant> cell) && !cell.HasPlant;
    }

    public bool TryPlant(Vector2Int position, TPlant plant)
    {
        return TryGetCell(position, out PlantingCell<TPlant> cell) && cell.TryPlant(plant);
    }

    public bool TryClearPlant(Vector2Int position)
    {
        return TryGetCell(position, out PlantingCell<TPlant> cell) && cell.TryClearPlant();
    }

    #endregion

    #region World Conversion

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        return grid.WorldToGridPosition(worldPosition);
    }

    public Vector3 GridToWorldCenter(Vector2Int position)
    {
        return grid.GridToWorldCenter(position);
    }

    #endregion

    #region Helpers

    private static List<Vector2Int> GetRectanglePositions(Vector2Int startPosition, Vector2Int size)
    {
        List<Vector2Int> positions = new();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                positions.Add(startPosition + new Vector2Int(x, y));
            }
        }

        return positions;
    }

    #endregion
}
