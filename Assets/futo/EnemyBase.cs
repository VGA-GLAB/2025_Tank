using Photon.Pun;
using UnityEngine;

/// <summary>
/// 敵の基本クラス
/// </summary>
public abstract class EnemyBase : MonoBehaviourPunCallbacks, ITank
{
    [Header("敵のステータス設定")]
    [SerializeField] private int _hp = 5;
    [SerializeField] private int _attack = 1;
    [SerializeField] private float _attackRange = 8;
    [SerializeField] private int _moveSpeed = 5;
    [SerializeField] private float _bulletInterval = 1.5f;

    [Header("ターゲット設定")]
    [SerializeField] private GameObject _player;

    public int Hp => _hp;
    public int AttackPower => _attack;
    public float AttackRange => _attackRange;
    public int MoveSpeed => _moveSpeed;
    public float BulletInterval => _bulletInterval;
    public GameObject Player => _player;

    
    private GameManager gameManager;
    protected virtual void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    public void Die()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            gameManager.GetComponent<PhotonView>().RPC("CheckEnemeyActive", RpcTarget.All);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    [PunRPC]
    public void Hit(int atk)
    {
        _hp -= atk;
        if(_hp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 一番近いプレイヤーをターゲットにする
    /// </summary>
    public void PlayerFind()
    {
        //TODO : GMからの取得に変える
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float nearestDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                _player = player;
            }
        }
    }

    /// <summary>
    /// 敵の移動処理
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// 敵の攻撃処理
    /// </summary>
    public abstract void Attack();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(_hp <= 0)
        {
            Debug.LogWarning($"{name}のHPが0以下です。1に修正します");
            _hp = 1;
        }

        if(_attack < 0)
        {
            Debug.LogWarning($"{name}の攻撃力が負の値です。0に修正します。");
            _attack = 0;
        }

        if(_attackRange < 0)
        {
            Debug.LogWarning($"{name}の攻撃可能範囲が0以下です。1に修正します。");
            _attackRange = 1;
        }

        if (_moveSpeed < 0)
        {
            Debug.LogWarning($"{name}の移動速度が負の値です。0に修正します。");
            _moveSpeed = 0;
        }

        if ( _bulletInterval <= 0)
        {
            Debug.LogWarning($"{name}の弾の発射間隔が0以下です。1に修正します。");
            _bulletInterval = 1;
        }
    }
#endif
}
