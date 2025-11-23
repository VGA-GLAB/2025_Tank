using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AttackIntervalGauge : MonoBehaviour
{
    [SerializeField] private Image _gauge;

    /// <summary>
    /// ゲージをセットします。
    /// </summary>
    /// <param name="gauge"></param>
    public void SetGauge(Image gauge)
    {
        _gauge = gauge;
    }

    /// <summary>
    /// ゲージのFillAmountをアニメーションで変更する
    /// </summary>
    /// <param name="value"></param>
    /// <param name="duration"></param>
    public void AnimateFillAmount(float value, float duration)
    {
        // ゲージを一旦リセットしてからアニメーションさせる
        _gauge.fillAmount = 0f;
        _gauge.DOFillAmount(value, duration);
    }
}
