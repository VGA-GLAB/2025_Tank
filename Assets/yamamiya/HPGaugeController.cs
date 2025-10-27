using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HPGaugeController : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private Image _hpImage;
    [SerializeField] private Image _burnImage;

    [Header("DOTweenの設定")]
    [SerializeField] private float _duration = 0.5f;
    [SerializeField, Tooltip("振動する強さ")] private float _strength = 10f;
    [SerializeField, Tooltip("振動数")] private int _vibrate = 100;

    [Header("HPゲージ演出の設定")]
    [SerializeField] private float _burnDelay = 0.5f;
    [SerializeField] private float _burnDurationDivisor = 2f;

    private ITank _tank;
    private float _startHP;

    private void Start()
    {
        if (_target != null)
        {
            SetTarget(_target);
        }
    }

    /// <summary>
    /// HPゲージの表示対象を設定し、
    /// 対象の初期HPに応じてゲージを初期化・更新。
    /// プレイヤーを生成するかボスエネミーを生成する際は読んでください。
    /// </summary>
    /// <param name="target">HPゲージの対象となるオブジェクト</param>
    public void SetTarget(GameObject target)
    {
        if (target.TryGetComponent(out _tank))
        {
            _startHP = _tank.Hp;
            UpdateHPGauge();
        }
    }

    /// <summary>
    /// HPゲージを現在のターゲットのHPに合わせて更新。
    /// </summary>
    public void UpdateHPGauge()
    {
        if (_tank == null || _startHP <= 0f)
        {
            return;
        }
        var burnDuraction = _duration / _burnDurationDivisor;
        if (_tank.Hp <= 0f)
        {
            GaugeEffect(0f, burnDuraction);
            return;
        }

        var value = _tank.Hp / _startHP;
        GaugeEffect(value, burnDuraction);
    }

    /// <summary>
    /// HPゲージとバーンゲージのアニメーション効果を適用させる。
    /// </summary>
    /// <param name="value">HPゲージの表示割合</param>
    /// <param name="burnDuraction">バーンゲージおよび振動の演出時間</param>
    private void GaugeEffect(float value, float burnDuraction)
    {
        // HPゲージのFillAmountをアニメーションで更新
        _hpImage.DOFillAmount(value, _duration)
                    .OnComplete(() =>
                    {
                        // HPゲージ更新後_burnDelay分遅らせてバーンゲージも同じ割合にアニメーション。
                        _burnImage.DOFillAmount(value, burnDuraction)
                        .SetDelay(_burnDelay);
                    });
        // HPゲージの更新に合わせて、ゲージ全体を振動させる
        this.transform.DOShakePosition(burnDuraction, _strength, _vibrate);
    }
}
