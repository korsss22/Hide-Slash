using System;
using UnityEngine;
using UnityEngine.UI;

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
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isHolding) return;

        currentHoldTime += Time.deltaTime;
        holdCircle.fillAmount = currentHoldTime / holdTime;
        Debug.Log(holdCircle.fillAmount);
    }

    public void StartHold()
    {
        isHolding = true;
        currentHoldTime = 0f;
        gameObject.SetActive(true);
    }

     public void CompleteHold()
    {
        isHolding = false;
        holdCircle.fillAmount = 1f;
        onHoldComplete?.Invoke();
        // 여기서 성공 이펙트 / 사운드
        // 필요하면 잠깐 보여주고 끄기
        gameObject.SetActive(false);
    }

    public void CancelHold()
    {
        isHolding = false;
        currentHoldTime = 0f;
        gameObject.SetActive(false);
    }
}
