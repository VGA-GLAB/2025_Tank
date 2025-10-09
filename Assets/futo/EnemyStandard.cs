using UnityEngine.AI;
using UnityEngine;

public class EnemyStandard : EnemyBase
{
    [SerializeField] private NavMeshAgent _agent;

    private float _distance;

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
        _distance = Vector3.Distance(transform.position, Player.transform.position);
        if (_distance > AttackRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(Player.transform.position);
        }
        else
        {
            _agent.isStopped = true;
        }
    }

    protected override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
