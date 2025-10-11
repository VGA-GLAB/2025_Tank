using UnityEngine;

public class PlayerTest : MonoBehaviour,ITank
{
    [SerializeField] private int _hp;
    [SerializeField] private int _atk;
    [SerializeField] private int _move;
    [SerializeField] private float _bullet;
    public int Hp => _hp;
    public int AttackPower => _atk;
    public int MoveSpeed => _move;
    public float BulletInterval => _bullet;

    public void Die()
    {
       
    }

    public void Hit(int atk)
    {
        _hp -= atk;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
