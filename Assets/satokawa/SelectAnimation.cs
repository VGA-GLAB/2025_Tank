using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectAnimation : ButtonScalerTween
{
    [SerializeField] private bool _enableLoopAnimation = true;
    [SerializeField] private bool _defaultStart = false;
    [SerializeField] private float _maxScale = 1.1f;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private Image _outLine;
    private RectTransform _rectTransform;
    private Vector3 _defaultScale;
    private Sequence _loopSequence;
    private bool _isHovered = false;
    public override void Start()
    {
        base.Start();
        _rectTransform = GetComponent<RectTransform>();
        _defaultScale = _rectTransform.localScale;

        if (_enableLoopAnimation)
            StartLoopAnimation();
        SetLoopAnimation(_defaultStart);
    }

    private void StartLoopAnimation()
    {
        if (!_enableLoopAnimation) return;
        if (_isHovered) return;

        _loopSequence?.Kill();
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
            _defaultScale = _rectTransform.localScale;
        }
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
        currentTween = rect.DOScale(originalScale, duration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // ループがONなら再開
                if (_enableLoopAnimation)
                    StartLoopAnimation();
            });

    }

    public void SetLoopAnimation(bool active)
    {
        _enableLoopAnimation = active;
        _outLine?.gameObject.SetActive(active);
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