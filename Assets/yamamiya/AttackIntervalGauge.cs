using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AttackIntervalGauge : MonoBehaviour
{
    [SerializeField] private Image _gauge;
    private Tween _fillTween;

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
        if(_fillTween != null)
        {
            _fillTween.Kill(true);
        }
        // ゲージを一旦リセットしてからアニメーションさせる
        _gauge.fillAmount = 0f;
        _fillTween = _gauge.DOFillAmount(value, duration);
    }

    public void aaa()
    {

        int[] nums = { 4, 3, 2, 3, 1 };

        string result = "";

        for (int i = nums.Length - 1; i >= 0; i--)
        {
            if (nums[i] == 3)
            {
                continue;
            }

            result += nums[i].ToString();
        }

        Debug.Log(result);



    }
}
public class AAAA : MonoBehaviour
{
    void Start()
    {
        Debug.Log(GetDoubleNumber(10));
    }
    int GetDoubleNumber(int num)
    {
        return num * 2;   
    }
}