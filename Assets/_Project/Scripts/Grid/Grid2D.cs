using System;
using System.Collections.Generic;
using UnityEngine;

public enum GridDirectionMode
{
    FourDirections,
    EightDirections
}

public sealed class Grid2D<T> where T : class
{
    #region Direction Offsets

    private static readonly Vector2Int[] FourDirectionOffsets =
    {
        new(0, 1),
        new(1, 0),
        new(0, -1),
        new(-1, 0)
    };

    private static readonly Vector2Int[] EightDirectionOffsets =
    {
        new(0, 1),
        new(1, 1),
        new(1, 0),
        new(1, -1),
        new(0, -1),
        new(-1, -1),
        new(-1, 0),
        new(-1, 1)
    };

    #endregion

    #region Fields

    private readonly T[,] cells;

    #endregion

    #region Properties

    public int Width { get; }
    public int Height { get; }
    public int CellCount => Width * Height;
    public float CellSize { get; }
    public Vector3 Origin { get; }

    #endregion

    #region Constructor

    public Grid2D(int width, int height, float cellSize = 1f, Vector3 origin = default)
    {
        Width = Mathf.Max(1, width);
        Height = Mathf.Max(1, height);
        CellSize = Mathf.Max(0.01f, cellSize);
        Origin = origin;

        cells = new T[Width, Height];
    }

    #endregion

    #region Bounds

    public bool IsInside(Vector2Int position)
    {
        return position.x >= 0 &&
               position.x < Width &&
               position.y >= 0 &&
               position.y < Height;
    }

    #endregion

    #region Cell Access

    public bool IsEmpty(Vector2Int position)
    {
        return TryGet(position, out T value) && value == null;
    }

    public T Get(Vector2Int position)
    {
        if (!IsInside(position))
        {
            throw new ArgumentOutOfRangeException(nameof(position), $"Grid position {position} is outside {Width}x{Height}.");
        }

        return cells[position.x, position.y];
    }

    public bool TryGet(Vector2Int position, out T value)
    {
        if (!IsInside(position))
        {
            value = null;
            return false;
        }

        value = cells[position.x, position.y];
        return true;
    }

    #endregion

    #region Cell Mutation

    public void Set(Vector2Int position, T value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!IsInside(position))
        {
            throw new ArgumentOutOfRangeException(nameof(position), $"Grid position {position} is outside {Width}x{Height}.");
        }

        cells[position.x, position.y] = value;
    }

    public bool TrySet(Vector2Int position, T value)
    {
        if (value == null || !IsInside(position))
        {
            return false;
        }

        cells[position.x, position.y] = value;
        return true;
    }

    #endregion

    #region Cell Validation

    public bool AreCellsInside(IEnumerable<Vector2Int> positions)
    {
        if (positions == null)
        {
            return false;
        }

        foreach (Vector2Int position in positions)
        {
            if (!IsInside(position))
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Placement

    public bool CanPlace(IEnumerable<Vector2Int> positions)
    {
        return CanUseCells(positions, value => value == null);
    }

    public bool CanUseCells(IEnumerable<Vector2Int> positions, Predicate<T> canUseCell)
    {
        if (positions == null || canUseCell == null)
        {
            return false;
        }

        foreach (Vector2Int position in positions)
        {
            if (!TryGet(position, out T value) || !canUseCell(value))
            {
                return false;
            }
        }

        return true;
    }

    public bool TryPlace(T value, IEnumerable<Vector2Int> positions)
    {
        if (value == null || !CanPlace(positions))
        {
            return false;
        }

        foreach (Vector2Int position in positions)
        {
            cells[position.x, position.y] = value;
        }

        return true;
    }

    #endregion

    #region Clearing

    public void Clear(Vector2Int position)
    {
        if (IsInside(position))
        {
            cells[position.x, position.y] = null;
        }
    }

    public void Clear(IEnumerable<Vector2Int> positions)
    {
        if (positions == null)
        {
            return;
        }

        foreach (Vector2Int position in positions)
        {
            Clear(position);
        }
    }

    public void ClearAll()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                cells[x, y] = null;
            }
        }
    }

    #endregion

    #region Iteration

    public IEnumerable<Vector2Int> GetPositions()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }

    public IEnumerable<T> GetValues()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                T value = cells[x, y];
                if (value != null)
                {
                    yield return value;
                }
            }
        }
    }

    #endregion

    #region Neighbors

    public List<Vector2Int> GetNeighborPositions(Vector2Int position, GridDirectionMode directionMode = GridDirectionMode.FourDirections)
    {
        Vector2Int[] offsets = directionMode == GridDirectionMode.EightDirections
            ? EightDirectionOffsets
            : FourDirectionOffsets;

        List<Vector2Int> neighbors = new();

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int neighbor = position + offset;
            if (IsInside(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    #endregion

    #region World Conversion

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - Origin;

        return new Vector2Int(
            Mathf.FloorToInt(localPosition.x / CellSize),
            Mathf.FloorToInt(localPosition.y / CellSize)
        );
    }

    public Vector3 GridToWorldCenter(Vector2Int position)
    {
        return Origin + new Vector3(
            position.x * CellSize + CellSize / 2f,
            position.y * CellSize + CellSize / 2f,
            0f
        );
    }

    public Vector3 GetCellsCenterWorld(IReadOnlyList<Vector2Int> positions)
    {
        if (positions == null || positions.Count == 0)
        {
            return Origin;
        }

        int minX = positions[0].x;
        int maxX = positions[0].x;
        int minY = positions[0].y;
        int maxY = positions[0].y;

        for (int i = 1; i < positions.Count; i++)
        {
            Vector2Int position = positions[i];
            minX = Mathf.Min(minX, position.x);
            maxX = Mathf.Max(maxX, position.x);
            minY = Mathf.Min(minY, position.y);
            maxY = Mathf.Max(maxY, position.y);
        }

        float centerX = (minX + maxX) / 2f * CellSize + CellSize / 2f;
        float centerY = (minY + maxY) / 2f * CellSize + CellSize / 2f;

        return Origin + new Vector3(centerX, centerY, 0f);
    }

    #endregion
}
