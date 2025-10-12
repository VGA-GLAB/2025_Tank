using UnityEngine;
using Photon.Pun;

public class EnemyStandard : EnemyBase
{
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
            _turret.transform.LookAt(_playerPosition);
        }

        _hasObject = false;
        if (Physics.Raycast(_rayOrigin, _direction, out RaycastHit hit, _attackRange))
        {
            if (hit.collider.gameObject != Player)
            {
                _hasObject = true;
            }
        }

#if UNITY_EDITOR
        Debug.DrawRay(_rayOrigin, _direction * _attackRange, _hasObject ? Color.red : Color.green);
#endif

        if (_distance > _attackRange || _hasObject)
        {
            _agent.isStopped = false;
            _agent.SetDestination(Player.transform.position);
        }
        else
        {
            _agent.isStopped = true;
            Attack();
        }
    }

    public override void Attack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= BulletInterval)
        {
            Debug.Log("こうげき！");
            GameObject newBullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _muzzlePosition.position, Quaternion.identity);
            newBullet.transform.forward = _muzzlePosition.forward;
            if (newBullet.TryGetComponent<BulletControl>(out BulletControl component))
            {
                component._attack = AttackPower;
            }
            _attackTimer = 0;
        }
    }
}