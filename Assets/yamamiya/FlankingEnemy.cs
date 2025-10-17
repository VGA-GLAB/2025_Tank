using Photon.Pun;
using UnityEngine;

public class FlankingEnemy : EnemyBase
{
    [Header("背後を取ろうとする時の設定")]
    [SerializeField] private float _flankOffset = 2f;
    [SerializeField, Tooltip("プレイヤーの背後の場所の更新")] private float _updateInterval = 0.5f;

    [SerializeField]private float _distance;
    private float _attackTimer;
    private float _updateTimer;
    private int flanDirection;
    private Vector3 _direction;
    private Vector3 _rayOrigin;
    private Vector3 _nowPosition;
    private Vector3 _playerPosition;
    private Vector3 _behindPosition;
    private Vector3 _lateraDir;
    private Vector3 _targetPosition;
    private bool _hasObject;

    protected override void Start()
    {
        base.Start();
        flanDirection = Random.value > 0.5f ? 1 : -1;
    }

    private void Update()
    {
        Move();
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

        _updateTimer -= Time.deltaTime;
        if (_updateTimer <= 0f)
        {
            _behindPosition = Player.transform.position - Player.transform.forward * _attackRange;
            _updateTimer = _updateInterval;
        }

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

        if (_distance > _attackRange ||_hasObject)
        {
            _agent.isStopped = false;
            MoveToFlankPosition();
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
        if(_attackTimer >= _bulletInterval)
        {
            GameObject newBullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _muzzlePosition.position, Quaternion.identity);
            newBullet.transform.forward = _muzzlePosition.forward;
            if(newBullet.TryGetComponent<BulletControl>(out BulletControl component))
            {
                component._attack = _attack;
            }
            _attackTimer = 0f;
        }
    }

    /// <summary>
    /// ターゲットの場所に回り込もうとする
    /// </summary>
    private void MoveToFlankPosition()
    {
        _lateraDir = Player.transform.right * flanDirection;
        _targetPosition = _behindPosition + _lateraDir * _flankOffset;

        _agent.SetDestination(_targetPosition);
    }
}
