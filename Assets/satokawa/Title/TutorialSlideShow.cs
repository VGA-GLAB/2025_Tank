using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideShow : MonoBehaviour
{
    [SerializeField] private Sprite[] images;
    [SerializeField] private Image mainImage;
    [SerializeField] private Image slideImage;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float movePositionX;
    private int viewIndex = 0;
    private bool isMove = false;
    private void OnEnable()
    {
        viewIndex = 0;
        mainImage.sprite = images[viewIndex];
        slideImage.gameObject.SetActive(false);
    }

    public void OnClick(bool next)
    {
        if (isMove) return;
        isMove = true;
        slideImage.sprite = images[viewIndex];
        slideImage.gameObject.SetActive(true);

        //indexを更新
        viewIndex = (viewIndex + (next ? 1 : -1) + images.Length) % images.Length;
        mainImage.sprite = images[viewIndex];
        mainImage.rectTransform.localPosition = new Vector3(movePositionX * (next ? 1 : -1), mainImage.rectTransform.localPosition.y, 0);
        slideImage.rectTransform.localPosition = Vector3.zero;

        mainImage.rectTransform.DOLocalMoveX(0, moveDuration);
        slideImage.rectTransform.DOLocalMoveX(movePositionX * (next ? -1 : 1), moveDuration)
            .OnComplete(() =>
            {
                slideImage.gameObject.SetActive(false);
                isMove = false;
            });
    }
}