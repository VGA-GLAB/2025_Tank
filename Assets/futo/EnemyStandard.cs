using UnityEngine.AI;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyStandard : EnemyBase
{
    [Header("敵の基礎設定")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private GameObject _bulletprehab;
    [SerializeField] private GameObject _battery;
    [SerializeField] private Transform _muzzlePosition;
  
    private float _distance;
    private float _attackTimer;
    private Vector3 _direction;
    private Vector3 _rayOrigin;
    private Vector3 _nowPosition;
    private bool _hasObject;   

    private void Start()
    {
        if(_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        _agent.speed = MoveSpeed;
    }

    //テスト用
    private void Update()
    {
        Move();
    }

    protected override void Move() 
    {
        _nowPosition = transform.position;
        _distance = Vector3.Distance(_nowPosition, Player.transform.position);
        _direction = (Player.transform.position - _nowPosition).normalized;
        _rayOrigin = _nowPosition + Vector3.up * 1.0f;

        _hasObject = false;

        _battery.transform.LookAt(Player.transform.position);

        if (Physics.Raycast(_rayOrigin, _direction, out RaycastHit hit, AttackRange))
        {
            if(hit.collider.gameObject != Player)
            {
                _hasObject = true;
            }
        }

#if UNITY_EDITOR
        Color rayColor = _hasObject ? Color.red : Color.green;
        Debug.DrawRay(_rayOrigin, _direction * AttackRange, rayColor);
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

    protected override void Attack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= BulletInterval)
        {
            Debug.Log("こうげき！");
            //:todo ここに弾の生成を入れる
            //Instantiate(_bulletprehab,new Vector3(_muzzlePosition.localPosition.x,_muzzlePosition.localPosition.y,_muzzlePosition.localPosition.z),Quaternion.identity);
            _attackTimer = 0;
        }
    }
}
