using Photon.Pun;
using UnityEngine;

public class FlankingEnemy : EnemyBase
{
    private float _attackTimer;
    protected override void Start()
    {
        base.Start();
    }

    public override void Move()
    {
        
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
}
