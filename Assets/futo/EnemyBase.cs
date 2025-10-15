using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
/// <summary>
/// 敵の基本クラス
/// </summary>
public abstract class EnemyBase : MonoBehaviourPunCallbacks, ITank
{
    [Header("敵のステータス設定")]
    [SerializeField] protected int _hp = 5;
    [SerializeField] protected int _attack = 1;
    [SerializeField] protected float _attackRange = 8;
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] protected float _bulletInterval = 1.5f;
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected GameObject _turret;
    [SerializeField] protected Transform _muzzlePosition;

    [Header("ターゲット設定")]
    [SerializeField] private GameObject _player;

    public int Hp => _hp;
    public int AttackPower => _attack;
    public float MoveSpeed => _moveSpeed;
    public GameObject Player => _player;

    private GameManager gameManager;
    protected NavMeshAgent _agent;
    protected virtual void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        _agent.speed = MoveSpeed;
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
    public void Hit(int attack)
    {
        _hp -= attack;
        if (_hp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    ///  一番近いプレイヤーをターゲットにする
    /// </summary>
    /// <returns>ture　みつかった false みつからなかった</returns>
    public bool PlayerFind()
    {
        if(gameManager?.Players == null)
        {
            return false;
        }
        List<PlayerController> players = gameManager.Players;
        float nearestDistance = Mathf.Infinity;

        foreach (PlayerController player in players)
        {
            if(player == null)
            {
                continue;
            }
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                _player = player.gameObject;
            }
        }
        return Player != null;
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
        if (_hp <= 0)
        {
            Debug.LogWarning($"{name}のHPが0以下です。1に修正します");
            _hp = 1;
        }

        if (_attack < 0)
        {
            Debug.LogWarning($"{name}の攻撃力が負の値です。0に修正します。");
            _attack = 0;
        }

        if (_attackRange < 0)
        {
            Debug.LogWarning($"{name}の攻撃可能範囲が0以下です。1に修正します。");
            _attackRange = 1;
        }

        if (_moveSpeed < 0)
        {
            Debug.LogWarning($"{name}の移動速度が負の値です。0に修正します。");
            _moveSpeed = 0;
        }

        if (_bulletInterval <= 0)
        {
            Debug.LogWarning($"{name}の弾の発射間隔が0以下です。1に修正します。");
            _bulletInterval = 1;
        }
    }
#endif
}
