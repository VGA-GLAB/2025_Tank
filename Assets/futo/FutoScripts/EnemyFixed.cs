using Photon.Pun;
using UnityEngine;

public class EnemyFixed : EnemyBase
{
    [SerializeField] private Transform[] _patrolPoint;
    [SerializeField] private string _patrolPointName;

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
            base.Attack();
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
        GameObject[] points = GameObject.FindGameObjectsWithTag(_patrolPointName);

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
