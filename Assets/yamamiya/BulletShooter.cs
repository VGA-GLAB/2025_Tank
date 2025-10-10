using UnityEngine;
using UnityEngine.InputSystem;

public class BulletShooter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shotPosition;
    [SerializeField] private Transform _turret;

    private int _atk;
    private float _bulletInterval;
    private float _intervalTimer;

    private void Start()
    {
        _intervalTimer = 0;
    }

    private void Update()
    {
        if (_intervalTimer > 0)
        {
            _intervalTimer -= Time.deltaTime;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        ShotBullet();
    }

    /// <summary>
    /// 攻撃力と砲弾の連射インターバルを設定
    /// </summary>
    /// <param name="atk">攻撃力</param>
    /// <param name="bulletInterval">砲弾の連射インターバル</param>
    public void IntializeAttackSettings(int atk, float bulletInterval)
    {
        _atk = atk;
        _bulletInterval = bulletInterval;
    }

    /// <summary>
    /// 砲弾を発射する
    /// </summary>
    private void ShotBullet()
    {
        if (_intervalTimer <= 0)
        {
            _intervalTimer = _bulletInterval;
            GameObject newBullet = Instantiate(_bulletPrefab, _shotPosition.position, _turret.rotation);
            newBullet.transform.forward = _turret.forward;
            
            if(newBullet.TryGetComponent<BulletControl>(out BulletControl component))
            {
                component._atk = _atk;
            }
        }
    }
}
