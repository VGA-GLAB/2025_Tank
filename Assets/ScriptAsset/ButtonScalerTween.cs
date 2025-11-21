using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // ← 忘れずに

public class ButtonScalerTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected float scaleUp = 1.1f;       // 拡大倍率
    [SerializeField] protected float duration = 0.2f;      //
                                          //アニメーションの時間

    protected Vector3 originalScale;
    protected Tween currentTween;

    public virtual void Start()
    {
        originalScale = transform.localScale;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill();

        // 拡大
        currentTween = transform.DOScale(originalScale * scaleUp, duration).SetEase(Ease.OutBack);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // 縮小
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }
}
