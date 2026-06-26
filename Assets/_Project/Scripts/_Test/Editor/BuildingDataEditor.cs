#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingData))]
public class BuildingDataEditor : Editor
{
    private const int CellSize = 24;

    private SerializedProperty descriptionProperty;
    private SerializedProperty costProperty;
    private SerializedProperty modelProperty;
    private SerializedProperty editorGridSizeProperty;
    private SerializedProperty occupiedCellsProperty;

    private void OnEnable()
    {
        descriptionProperty = serializedObject.FindProperty("Description");
        costProperty = serializedObject.FindProperty("Cost");
        modelProperty = serializedObject.FindProperty("Model");
        editorGridSizeProperty = serializedObject.FindProperty("EditorGridSize");
        occupiedCellsProperty = serializedObject.FindProperty("OccupiedCells");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(descriptionProperty);
        EditorGUILayout.PropertyField(costProperty);
        EditorGUILayout.PropertyField(modelProperty);
        EditorGUILayout.PropertyField(editorGridSizeProperty);

        Vector2Int gridSize = editorGridSizeProperty.vector2IntValue;
        gridSize.x = Mathf.Max(1, gridSize.x);
        gridSize.y = Mathf.Max(1, gridSize.y);
        editorGridSizeProperty.vector2IntValue = gridSize;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Occupied Cells", EditorStyles.boldLabel);
        DrawCellsGrid(gridSize);

        EditorGUILayout.Space(8);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Clear"))
            {
                occupiedCellsProperty.ClearArray();
            }

            if (GUILayout.Button("Select 1 Cell"))
            {
                occupiedCellsProperty.ClearArray();
                occupiedCellsProperty.InsertArrayElementAtIndex(0);
                occupiedCellsProperty.GetArrayElementAtIndex(0).vector2IntValue = Vector2Int.zero;
            }
        }

        EditorGUILayout.PropertyField(occupiedCellsProperty);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCellsGrid(Vector2Int gridSize)
    {
        HashSet<Vector2Int> selectedCells = GetSelectedCells();

        for (int y = gridSize.y - 1; y >= 0; y--)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(y.ToString(), GUILayout.Width(18));

                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector2Int cell = new(x, y);
                    bool selected = selectedCells.Contains(cell);

                    Color oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = selected ? Color.green : Color.gray;

                    if (GUILayout.Button("", GUILayout.Width(CellSize), GUILayout.Height(CellSize)))
                    {
                        ToggleCell(cell, selected);
                    }

                    GUI.backgroundColor = oldColor;
                }
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(22);
            for (int x = 0; x < gridSize.x; x++)
            {
                GUILayout.Label(x.ToString(), GUILayout.Width(CellSize));
            }
        }
    }

    private HashSet<Vector2Int> GetSelectedCells()
    {
        HashSet<Vector2Int> cells = new();

        for (int i = 0; i < occupiedCellsProperty.arraySize; i++)
        {
            cells.Add(occupiedCellsProperty.GetArrayElementAtIndex(i).vector2IntValue);
        }

        return cells;
    }

    private void ToggleCell(Vector2Int cell, bool selected)
    {
        if (selected)
        {
            for (int i = 0; i < occupiedCellsProperty.arraySize; i++)
            {
                if (occupiedCellsProperty.GetArrayElementAtIndex(i).vector2IntValue == cell)
                {
                    occupiedCellsProperty.DeleteArrayElementAtIndex(i);
                    return;
                }
            }
        }

        occupiedCellsProperty.InsertArrayElementAtIndex(occupiedCellsProperty.arraySize);
        occupiedCellsProperty.GetArrayElementAtIndex(occupiedCellsProperty.arraySize - 1).vector2IntValue = cell;
    }
}
#endif
