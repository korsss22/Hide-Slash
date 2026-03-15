using UnityEngine;

[CreateAssetMenu(fileName = "PropData", menuName = "Scriptable Objects/PropData")]
public class PropIdentify : ScriptableObject
{
    public int propID;
    public GameObject propPrefab;
}
