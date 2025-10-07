using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour, ITank
{
    [SerializeField] private int _hp;
    [SerializeField] private int _atk;
    [SerializeField] private int _moveSpeed;
    [SerializeField] private int _bulletInterval;

    [SerializeField] private GameObject _player;

    public int Hp => _hp;
    public int ATK => _atk;
    public int MoveSpeed => _moveSpeed;
    public int BulletInterval => _bulletInterval;
    public GameObject Player => _player;

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public void Hit(int atk)
    {
        _hp -= atk;
        if(_hp < 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 敵の移動処理
    /// </summary>
    protected abstract void EnemyMove();

    /// <summary>
    /// 敵の攻撃処理
    /// </summary>
    protected abstract void EnemyAttack();
}
