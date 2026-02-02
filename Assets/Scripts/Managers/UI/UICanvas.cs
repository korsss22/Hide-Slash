using UnityEngine;

public class UICanvas : MonoBehaviour
{
    void Start()
    {
        Debug.Log("initializing UICanvas...");
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
    }
}
