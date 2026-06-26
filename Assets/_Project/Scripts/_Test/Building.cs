using UnityEngine;

public class Building : MonoBehaviour
{
    private BuildingModel model;
    private BuildingData data;

    public string Description => data.Description;
    public int Cost => data.Cost;

    public void Setup(BuildingData data, float rotation)
    {
        this.data = data;
        model = Instantiate(data.Model,transform.position,Quaternion.identity,transform);
        model.SetRotation(rotation); 
    }
}
