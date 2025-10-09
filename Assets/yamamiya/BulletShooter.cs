using UnityEngine;
using UnityEngine.InputSystem;

public class BulletShooter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shotPosition; //TODO マウスのカーソルに合わせて飛んでいくようにしたい

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

    /// <summary>
    /// 砲弾の連射インターバルを設定
    /// </summary>
    /// <param name="bulletInterval">砲弾の連射インターバル</param>
    public void SetBulletInterval(float bulletInterval)
    {
        _bulletInterval = bulletInterval;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        ShotBullet();
    }

    /// <summary>
    /// 砲弾を発射する
    /// </summary>
    private void ShotBullet()
    {
        if (_intervalTimer <= 0)
        {
            _intervalTimer = _bulletInterval;
            GameObject newBullet = Instantiate(_bulletPrefab, _shotPosition.position, Quaternion.identity);
            newBullet.transform.forward = this.transform.forward;
            // TODO 砲弾に攻撃力を渡す処理
        }
    }
}
