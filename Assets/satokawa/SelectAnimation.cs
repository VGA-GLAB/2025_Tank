using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectAnimation : ButtonScalerTween
{
    public bool enableLoopAnimation = true; 
    public float _maxScale = 1.1f;
    public float _duration = 0.5f;

    private RectTransform _rectTransform;
    private Vector3 _defaultScale;
    private Sequence _loopSequence;
    private bool _isHovered = false;

    public override void Start()
    {
        base.Start();
        _rectTransform = GetComponent<RectTransform>();
        _defaultScale = _rectTransform.localScale;

        if (enableLoopAnimation)
            StartLoopAnimation();

        SetLoopAnimation(false);
    }

    private void StartLoopAnimation()
    {
        if (!enableLoopAnimation) return;
        if (_isHovered) return;

        _loopSequence?.Kill();
        _rectTransform.localScale = _defaultScale;

        _loopSequence = DOTween.Sequence();
        _loopSequence
            .Append(_rectTransform.DOScale(_defaultScale * _maxScale, _duration))
            .Append(_rectTransform.DOScale(_defaultScale, _duration))
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopLoopAnimation()
    {
        _loopSequence?.Kill();
        _rectTransform.localScale = _defaultScale;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        StopLoopAnimation(); // ← ホバー時はループ止める
        base.OnPointerEnter(eventData); // ButtonScalerTween の挙動を維持
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;

        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // ループがONなら再開
                if (enableLoopAnimation)
                    StartLoopAnimation();
            });

    }

    public void SetLoopAnimation(bool active)
    {
        enableLoopAnimation = active;

        if (!active)
        {
            StopLoopAnimation();
        }
        else if (!_isHovered)
        {
            StartLoopAnimation();
        }
    }
}
