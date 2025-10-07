using UnityEngine.AI;
using UnityEngine;

public class EnemyStandard : EnemyBase
{
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] float _attackRange;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = MoveSpeed;
    }
    //テスト用
    private void Update()
    {
        Move();
    }

    protected override void Move()
    {
        _agent.SetDestination(Player.transform.position);
    }

    protected override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
