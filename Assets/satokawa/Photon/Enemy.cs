using Photon.Pun;
using UnityEngine;

/// <summary>
///　TODO: テスト用なのでいつか消す
/// </summary>
public class Enemy : MonoBehaviourPunCallbacks,ITank
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
        PhotonNetwork.Destroy(this.gameObject);
    }
    [PunRPC]
    public void Hit(int atk)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        _hp -= atk;
        if(_hp <= 0)
        {
            Die();
        }
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
