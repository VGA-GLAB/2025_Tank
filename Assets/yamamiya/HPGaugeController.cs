using UnityEngine;
using UnityEngine.UI;

public class HPGaugeController : MonoBehaviour
{
    [SerializeField] private Image _hpGuage;
    private float _startHP;

    private void Update()
    {
        this.transform.rotation = Camera.main.transform.rotation;
    }

    /// <summary>
    /// 開始時点のHPを設定する
    /// </summary>
    /// <param name="hp"></param>
    public void SetStartHP(float hp)
    {
        _startHP = hp;
        UpdateHPGuage(hp);
    }

    /// <summary>
    /// HPゲージを更新する 
    /// </summary>
    /// <param name="hp"></param>
    public void UpdateHPGuage(float hp)
    {
        _hpGuage.fillAmount = hp / _startHP;
    }
}
