using System.Collections;
using UnityEngine;

public class Tester : MonoBehaviour
{
    void Start()
    {
        
    }

    [ContextMenu("Test Debug Queue")]
    public void TestMessage() {
        StartCoroutine(TestMessageQueue());
    }

    private IEnumerator TestMessageQueue() {
        yield return null;
        for (int i = 0; i < 10; i++) {
            UIManager.PrintUI(DEBUG_TYPE.ERROR, "it's test"+i);
        }
    }
}
