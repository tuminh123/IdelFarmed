using UnityEngine;

public class BuildingModel : MonoBehaviour
{
    [SerializeField] private Transform wapper;
    public float Rotation => wapper.transform.eulerAngles.z; // 3D:wapper.transform.eulerAngles.y

    public void SetRotation(float rotation) => wapper.transform.rotation = Quaternion.Euler(0,0,rotation);
}
