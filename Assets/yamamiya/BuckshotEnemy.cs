using Photon.Pun;
using UnityEngine;

public class BuckshotEnemy : EnemyBase
{
    [Header("散弾の左右の弾の撃つ角度")]
    [SerializeField] float _buckShotAngle;
    [SerializeField] private float _reboundAngle = 10f;
    [SerializeField] private float _reboundRayDistance = 1.5f;
    private float _distance;
    private float _attackTimer;
    private Vector3 _direction;
    private Vector3 _rayOrigin;
    private Vector3 _nowPosition;
    private Vector3 _playerPosition;
    private bool _hasObject;

    protected override void Start()
    {
        base.Start();
    }

    public override void Move()
    {
        if (!photonView.IsMine) return;

        if (Player == null)
        {
            if (!PlayerFind())
            {
                //みつからなかった時の処理
                return;
            }
        }
        _nowPosition = transform.position;
        _playerPosition = Player.transform.position;

        _distance = Vector3.Distance(_nowPosition, _playerPosition);
        _direction = (_playerPosition - _nowPosition).normalized;
        _rayOrigin = _nowPosition + Vector3.up * 1.0f;

        if (_turret != null)
        {
            //_turret.transform.LookAt(_playerPosition);
            Vector3 localHit = _turret.transform.parent.InverseTransformPoint(_playerPosition);
            Vector3 localDir = localHit - _turret.transform.localPosition;

            // 水平距離と高さを求めてピッチ角を計算
            float horizontalDistance = new Vector2(localDir.z, localDir.y).magnitude;
            if (horizontalDistance > 0.001f)
            {
                float angleX = -Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

                // X軸だけ回転
                _turret.transform.localRotation = Quaternion.Euler(angleX, 0f, 0f);
            }
        }

        if (_distance < _attackRange)
        {
            bool isPlayerHit = IsPlayerRayHit(0, _attackRange, true)
                && IsPlayerRayHit(_reboundAngle, _reboundRayDistance)
                && IsPlayerRayHit(-_reboundAngle, _reboundRayDistance);

            if (isPlayerHit)
            {
                _agent.isStopped = true;
                Attack();
                return;
            }
        }

        _agent.isStopped = false;
        _agent.SetDestination(Player.transform.position);
    }

    public override void Attack()
    {
        
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _bulletInterval)
        {
            base.Attack();
            // 正面方向に弾を生成
            GenerateBullet(_muzzlePosition.forward);

            // 左方向に角度を指定
            Quaternion leftAngle = Quaternion.Euler(0, -_buckShotAngle, 0);
            // 正面方向から左に角度をつけて生成
            GenerateBullet(leftAngle * _muzzlePosition.forward);

            // 右方向に角度を指定
            Quaternion rightAngle = Quaternion.Euler(0, _buckShotAngle, 0);
            // 正面方向から右に角度をつけて生成
            GenerateBullet(rightAngle * _muzzlePosition.forward);

            _attackTimer = 0f;
        }
    }

    /// <summary>
    /// 弾の生成
    /// </summary>
    /// <param name="direction">弾の方向</param>
    private void GenerateBullet(Vector3 direction)
    {
        GameObject newBullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _muzzlePosition.position, Quaternion.identity);
        newBullet.transform.forward = direction;
        if (newBullet.TryGetComponent(out BulletControl component))
        {
            component.SetBulletData(_attack, BulletControl.Target.Enemy, photonView);
        }
    }
}
