using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public const float CellSize = 1;
    [SerializeField] private BuildingData buildingData;
    [SerializeField] private BuildingPreview buildingPreview;
    [SerializeField] private Building buildingPrefabs;
    [SerializeField] private GridBuild grid;

    private BuildingPreview preview;
    private int previewRotation;

    void Update()
    {
        Vector3 mousePos = GetMouseWorldPosition();

        if(preview != null)
        {
            HandlePreview(mousePos);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                previewRotation = 0;
                preview = CreatePreview(buildingData,mousePos);
            }
        }
    }

    private void HandlePreview(Vector3 mousePos)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            previewRotation = (previewRotation + 90) % 360;
        }

        Vector2Int anchorCell = grid.WorldToGridPosition(mousePos);
        List<Vector2Int> buildPositions = GetBuildingPositions(preview.Data, anchorCell, previewRotation);
        bool canBuild = buildPositions.Count > 0 && grid.CanBuild(buildPositions);

        grid.ShowPreview(buildPositions, canBuild);

        preview.transform.position = buildPositions.Count > 0
            ? grid.GetCellsCenterWorld(buildPositions)
            : grid.GetCellCenterWorld(anchorCell);
        preview.SetRotation(previewRotation);

        if (canBuild)
        {
            preview.ChangeState(BuildingPreviewState.POSITIVE);

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding(buildPositions);
            }
        }
        else
        {
            preview.ChangeState(BuildingPreviewState.NEGATIVE);
        }
    }

    private void PlaceBuilding(List<Vector2Int> buildingPositions)
    {
        if (!grid.CanBuild(buildingPositions)) return;

        Building building = Instantiate(buildingPrefabs, preview.transform.position, Quaternion.identity);
        building.Setup(preview.Data, previewRotation);
        grid.SetBuilding(building, buildingPositions);
        grid.ClearPreview();
        Destroy(preview.gameObject);
        preview = null;
    }

    private List<Vector2Int> GetBuildingPositions(BuildingData data, Vector2Int anchorCell, int rotation)
    {
        List<Vector2Int> cells = new();
        if (data == null || data.OccupiedCells == null) return cells;

        foreach (Vector2Int localCell in data.OccupiedCells)
        {
            cells.Add(anchorCell + RotateCell(localCell, rotation));
        }

        return cells;
    }

    private Vector2Int RotateCell(Vector2Int cell, int rotation)
    {
        rotation = ((rotation % 360) + 360) % 360;

        return rotation switch
        {
            90 => new Vector2Int(-cell.y, cell.x),
            180 => new Vector2Int(-cell.x, -cell.y),
            270 => new Vector2Int(cell.y, -cell.x),
            _ => cell
        };
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0f;

        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private BuildingPreview CreatePreview(BuildingData data,Vector3 pos)
    {
        BuildingPreview previewInstance = Instantiate(buildingPreview, pos, Quaternion.identity);
        previewInstance.Setup(data);
        return previewInstance;
    }
}
