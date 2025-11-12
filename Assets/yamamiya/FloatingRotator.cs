using UnityEngine;
using DG.Tweening;

public class FloatingRotator : MonoBehaviour
{
    [Header("上下移動設定")]
    [SerializeField] private Vector3 _floatOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private float _floatDuration = 1f;

    [Header("回転設定")]
    [SerializeField] private float _rotateDuration = 1f;
    [SerializeField] private float _rotateDelay = 0.1f;
    void Start()
    {
        DOTween.Sequence()
            .Append(this.transform.DOMove(_floatOffset, _floatDuration)
            .SetRelative(true)
            .SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo);

        this.transform.DORotate(new Vector3(0f, 360f, 0f), _rotateDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetDelay(_rotateDelay)
            .SetLoops(-1);
    }

    private void OnDestroy()
    {
        this.transform.DOKill();
    }
}
