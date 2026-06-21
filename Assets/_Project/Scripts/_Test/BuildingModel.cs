using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingModel : MonoBehaviour
{
    [SerializeField] private Transform wapper;
    public float Rotation => wapper.transform.eulerAngles.z; // 3D:wapper.transform.eulerAngles.y
    private BuildingShapeUnit[] buildingShapeUnits;

    private void Awake() {
        buildingShapeUnits = GetComponentsInChildren<BuildingShapeUnit>();
    }

    public void Rotate(float rotationStep) => wapper.Rotate(new Vector3(0,0,rotationStep));
    
    public List<Vector3> GetAllBuildingPosition() => buildingShapeUnits.Select(unit => unit.transform.position).ToList();
}
