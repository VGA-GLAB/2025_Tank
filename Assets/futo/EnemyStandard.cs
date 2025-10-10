using UnityEngine.AI;
using UnityEngine;

public class EnemyStandard : EnemyBase
{
    [Header("敵の基礎設定")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _battery;
    [SerializeField] private Transform _muzzlePosition;
  
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

        if(_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        _agent.speed = MoveSpeed;
    }

    public override void Move() 
    {
            if(Player ==  null)
        {
            PlayerFind();
        }
        _nowPosition = transform.position;
        _playerPosition = Player.transform.position;

        _distance = Vector3.Distance(_nowPosition, _playerPosition);
        _direction = (_playerPosition - _nowPosition).normalized;
        _rayOrigin = _nowPosition + Vector3.up * 1.0f;

        if(_battery != null)
        {
            _battery.transform.LookAt(_playerPosition);
        }

        _hasObject = false;
        if (Physics.Raycast(_rayOrigin, _direction, out RaycastHit hit, AttackRange))
        {
            if(hit.collider.gameObject != Player)
            {
                _hasObject = true;
            }
        }

#if UNITY_EDITOR
        Debug.DrawRay(_rayOrigin, _direction * AttackRange, _hasObject ? Color.red : Color.green);
#endif

        if (_distance > AttackRange || _hasObject)
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
            GameObject newBullet = Instantiate(_bulletPrefab,_muzzlePosition.position,Quaternion.identity);
            if(newBullet.TryGetComponent<BulletControl>(out BulletControl component))
            {
                component._attack = AttackPower;
            }
            _attackTimer = 0;
        }
    }
}
