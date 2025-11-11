using Photon.Pun;
using UnityEngine;

public class EnemyFixed : EnemyBase
{
    [SerializeField]　private Transform[] _patrolPoint;

    private int _destpoint = 0;
    private float _attackTimer;
    private Vector3 _playerPosition;


    protected override void Start()
    {
        base.Start();
        FindPatrolPoint();
        if( _patrolPoint.Length > 0 )
        {
            GoNextPoint();
        }
        else
        {
            Debug.Log("PatrolPointが見つかりません");
        }
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

        _playerPosition = Player.transform.position;


        if (_turret != null)
        {
            _turret.transform.LookAt(_playerPosition);
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            GoNextPoint();
        }

        Attack();
    }

    public override void Attack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _bulletInterval)
        {
            Debug.Log("こうげき！");
            GameObject newBullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _muzzlePosition.position, Quaternion.identity);
            newBullet.transform.forward = _muzzlePosition.forward;
            if (newBullet.TryGetComponent<BulletControl>(out BulletControl component))
            {
                component.SetBulletData(_attack, BulletControl.Target.Enemy);
            }
            _attackTimer = 0;
        }
    }

   　private void GoNextPoint()
    {
        if (_patrolPoint.Length == 0) return;

        if (!_agent.SetDestination(_patrolPoint[_destpoint].position))
        {
            Debug.LogError("経路探索失敗");
        }

        _destpoint = (_destpoint + 1) % _patrolPoint.Length;
    }

    private void FindPatrolPoint()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("PatrolPoint");

        if (points.Length > 0)
        {
            _patrolPoint = new Transform[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                _patrolPoint[i] = points[i].transform;
            }
        }
    }
}
