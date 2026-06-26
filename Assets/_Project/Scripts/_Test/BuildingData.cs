using UnityEngine;

[CreateAssetMenu(menuName ="DataSO/Building")]
public class BuildingData : ScriptableObject
{
    public string Description;
    public int Cost;
    public BuildingModel Model;
    public Vector2Int EditorGridSize = new(5, 5);
    public Vector2Int[] OccupiedCells = { Vector2Int.zero };
}
