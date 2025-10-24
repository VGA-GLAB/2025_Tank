using UnityEngine;
using UnityEngine.UI;

public class HPGaugeController : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private Image _hpGauge;
    private Camera _camera;
    private ITank _tank;
    private float _startHP;

    private void Start()
    {
        _camera = Camera.main;
        if (_target != null)
        {
            SetTarget(_target);
        }
    }

    private void Update()
    {
        this.transform.rotation = _camera.transform.rotation;
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
    /// HPゲージを現在のHP似合わせて更新。
    /// </summary>
    public void UpdateHPGauge()
    {
        if (_tank == null || _startHP <= 0f)
        {
            return;
        }

        if (_tank.Hp <= 0f)
        {
            _hpGauge.fillAmount = 0f;
            return;
        }
        _hpGauge.fillAmount = _tank.Hp / _startHP;
    }
}
