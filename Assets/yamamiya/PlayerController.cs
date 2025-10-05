using UnityEngine;

public class PlayerController : MonoBehaviour, ITank
{
    [SerializeField] private int _hp;
    [SerializeField] private int _atk;
    [SerializeField] private int _moveSpeed;
    [SerializeField] private int _bulletInterval;

    public int Hp => _hp;
    public int ATK => _atk;
    public int MoveSpeed => _moveSpeed;
    public int BulletInterval => _bulletInterval;

    public void Die()
    {

    }

    public void Hit(int atk)
    {

    }
}
