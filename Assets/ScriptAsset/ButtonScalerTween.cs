using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using UnityEditor; // ← 忘れずに

public class ButtonScalerTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected float scaleUp = 1.1f;       // 拡大倍率
    [SerializeField] protected float duration = 0.2f;      //アニメーションの時間
    [SerializeField] protected Button button;

    protected Vector3 originalScale;
    protected Tween currentTween;
    protected RectTransform rect;
    public void OnValidate()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }
    public virtual void Start()
    {
        originalScale = transform.localScale;
        rect = GetComponent<RectTransform>();
        if(button == null)
        {
            button = GetComponent<Button>();
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        currentTween?.Kill();

        // 拡大
        currentTween = rect.DOScale(originalScale * scaleUp, duration).SetEase(Ease.OutBack);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // 縮小
        currentTween?.Kill();
        currentTween = rect.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }
}
