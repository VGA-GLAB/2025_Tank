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

    private void Update()
    {
        _agent.SetDestination(Player.transform.position);
    }

    protected override void EnemyMove()
    {
        throw new System.NotImplementedException();
    }

    protected override void EnemyAttack()
    {
        throw new System.NotImplementedException();
    }
}
