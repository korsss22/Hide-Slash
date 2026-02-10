using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PropDatabase", menuName = "Scriptable Objects/PropDatabase")]
public class PropDatabase : ScriptableObject
{
    public List<PropIdentify> propDatas = new List<PropIdentify>();
}
