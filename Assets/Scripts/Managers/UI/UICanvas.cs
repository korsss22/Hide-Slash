using UnityEngine;

public class UICanvas : MonoBehaviour
{
    void Start()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
    }
}
