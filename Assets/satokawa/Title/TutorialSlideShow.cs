using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideShow : MonoBehaviour
{
    [SerializeField] private Sprite[] _images;
    [SerializeField] private Image _mainImage;
    [SerializeField] private Image _slideImage;
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private float _movePositionX;
    private int _viewIndex = 0;
    private bool _isMove = false;
    private void OnEnable()
    {
        _viewIndex = 0;
        _mainImage.sprite = _images[_viewIndex];
        _slideImage.gameObject.SetActive(false);
    }
    /// <summary>
    /// スライドを切り替える
    /// </summary>
    /// <param name="next">true 次　false 戻す</param>
    public void OnClick(bool next)
    {
        if (_isMove) return;

        //Sound:ページをめくる音
        CRIAudioManager.SE.Play("UI", "UI_slide");
        _isMove = true;
        _slideImage.sprite = _images[_viewIndex];
        _slideImage.gameObject.SetActive(true);

        //indexを更新
        _viewIndex = (_viewIndex + (next ? 1 : -1) + _images.Length) % _images.Length;
        _mainImage.sprite = _images[_viewIndex];
        _mainImage.rectTransform.localPosition = new Vector3(_movePositionX * (next ? 1 : -1), _mainImage.rectTransform.localPosition.y, 0);
        _slideImage.rectTransform.localPosition = Vector3.zero;

        _mainImage.rectTransform.DOLocalMoveX(0, _moveDuration);
        _slideImage.rectTransform.DOLocalMoveX(_movePositionX * (next ? -1 : 1), _moveDuration)
            .OnComplete(() =>
            {
                _slideImage.gameObject.SetActive(false);
                _isMove = false;
            });
    }
}