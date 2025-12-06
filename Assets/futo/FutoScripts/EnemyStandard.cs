using UnityEngine;
using Photon.Pun;
using System.Linq;

public class EnemyStandard : EnemyBase
{
    [SerializeField] private float _reboundAngle = 10f;
    [SerializeField] private float _reboundRayDistance = 10f;
    private float _distance;
    private float _attackTimer;
    private Vector3 _nowPosition;
    private Vector3 _playerPosition;

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
            GameObject newBullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _muzzlePosition.position, Quaternion.identity);
            newBullet.transform.forward = _muzzlePosition.forward;
            if (newBullet.TryGetComponent<BulletControl>(out BulletControl component))
            {
                component.SetBulletData(_attack, BulletControl.Target.Enemy, photonView);
            }
            _attackTimer = 0;
        }
    }
}