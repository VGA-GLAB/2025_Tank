using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // �� �Y�ꂸ��

public class ButtonScalerTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected float scaleUp = 1.1f;       // �g��{��
    [SerializeField] protected float duration = 0.2f;      //
                                          //�A�j���[�V�����̎���

    private Vector3 originalScale;
    private Tween currentTween;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill();

        // �g��
        currentTween = transform.DOScale(originalScale * scaleUp, duration).SetEase(Ease.OutBack);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // �k��
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }
}
