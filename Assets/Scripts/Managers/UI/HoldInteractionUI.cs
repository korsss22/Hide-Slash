using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HoldInteractionUI : MonoBehaviour
{
    private Image holdCircle;
    public float holdTime = 1f;
    public float currentHoldTime = 0f;
    public Action onHoldComplete;
    public bool isHolding = false;

    private void Awake()
    {
        holdCircle = GetComponent<Image>();
    }

    void Update()
    {
        if (!isHolding) return;

        currentHoldTime += Time.deltaTime;
        holdCircle.fillAmount = currentHoldTime / holdTime;
    }

    public void StartHold(Action onLoaded)
    {
        isHolding = true;
        currentHoldTime = 0f;
        onHoldComplete += onLoaded;
        gameObject.SetActive(true);
    }

    public void CompleteHold()
    {
        isHolding = false;
        holdCircle.fillAmount = 1f;
        onHoldComplete?.Invoke();
        gameObject.SetActive(false);
    }

    public void CancelHold()
    {
        isHolding = false;
        currentHoldTime = 0f;
        gameObject.SetActive(false);
    }
}
