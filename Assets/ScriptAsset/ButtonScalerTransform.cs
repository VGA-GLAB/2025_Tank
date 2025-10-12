using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScalerBounce : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleUp = 1.1f;  // 最大倍率
    [SerializeField] private float duration = 0.3f; // アニメーション時間
    [SerializeField] private float bounce = 0.1f;   // バウンス幅

    private Vector3 originalScale;
    private Vector3 targetScale;
    private float t;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleUp;
        t = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        t = 0f;
    }

    private void Update()
    {
        if (transform.localScale != targetScale)
        {
            t += Time.deltaTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // バウンス用サイン波
            float bounceOffset = Mathf.Sin(smoothT * Mathf.PI) * bounce;

            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, smoothT)
                                   + Vector3.one * bounceOffset;
        }
    }
}

