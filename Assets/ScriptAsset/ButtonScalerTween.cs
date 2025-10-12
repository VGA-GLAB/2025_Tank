using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // © –Y‚ê‚¸‚É

public class ButtonScalerTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected float scaleUp = 1.1f;       // Šg‘å”{—¦
    [SerializeField] protected float duration = 0.2f;      //
                                          //ƒAƒjƒ[ƒVƒ‡ƒ“‚ÌŠÔ

    private Vector3 originalScale;
    private Tween currentTween;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill();

        // Šg‘å
        currentTween = transform.DOScale(originalScale * scaleUp, duration).SetEase(Ease.OutBack);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // k¬
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }
}
