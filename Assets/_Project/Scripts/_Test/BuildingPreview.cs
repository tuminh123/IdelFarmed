using System.Collections.Generic;
using UnityEngine;

public enum BuildingPreviewState
{
    POSITIVE = 0,
    NEGATIVE = 1,
}

public class BuildingPreview : MonoBehaviour
{
    [SerializeField] private Material positiveMaterial;
    [SerializeField] private Material negativeMaterial;

    public BuildingPreviewState State { get; private set; } = BuildingPreviewState.NEGATIVE;
    public BuildingData Data { get; private set; }
    public BuildingModel BuildingModel { get; private set; }

    private List<Renderer> renderers = new();
    private List<Collider2D> collider2Ds = new();

    public void Setup(BuildingData data)
    {
        Data = data;
        BuildingModel = Instantiate(data.Model, transform.position, Quaternion.identity, transform);
        renderers.AddRange(BuildingModel.GetComponentsInChildren<Renderer>());
        collider2Ds.AddRange(BuildingModel.GetComponentsInChildren<Collider2D>());

        foreach (var col in collider2Ds)
        {
            col.enabled = false;
        }

        SetPreviewMaterial(State);
    }

    public void ChangeState(BuildingPreviewState newState)
    {
        if (newState == State) return;
        State = newState;
        SetPreviewMaterial(newState);
    }
    
    public void SetRotation(float rotation)
    {
        BuildingModel.SetRotation(rotation);
    }

    private void SetPreviewMaterial(BuildingPreviewState newState)
    {
        Material previewMat = newState == BuildingPreviewState.POSITIVE ? positiveMaterial : negativeMaterial;

        foreach (var rend in renderers)
        {
            Material[] mats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = previewMat;
            }
            rend.materials = mats;
        }
    }

   
}
