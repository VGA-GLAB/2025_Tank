using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;
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
    [SerializeField] protected HPGaugeController _hpGauge;
    [SerializeField] protected Animator _animator;
    [SerializeField] private ParticleSystem killEffect;
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
        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _agent.speed = MoveSpeed;
    }
    public void Die()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            if(killEffect != null)
            {
                 Instantiate(killEffect.gameObject, this.transform.position, Quaternion.identity);
            }

            gameManager.GetComponent<PhotonView>().RPC("CheckEnemeyActive", RpcTarget.All);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    [PunRPC]
    public void Hit(int attack,int viewID)
    {
        _hp -= attack;

        PhotonView attackerView = PhotonView.Find(viewID);
        if(attackerView == null)
        {
            Debug.LogError("don't find photonview");
        }

        if(attackerView.TryGetComponent(out PlayerController controller))
        {
            Debug.Log("ChangeTarget");
            _player = controller.gameObject;
        }


        if (_hp <= 0)
        {
            Die();
        }
        _hpGauge.UpdateHPGauge(true);

    }
    /// <summary>
    ///  一番近いプレイヤーをターゲットにする
    /// </summary>
    /// <returns>ture　みつかった false みつからなかった</returns>
    protected virtual bool PlayerFind(int n = 1)
    {
        if (gameManager?.Players == null || gameManager.Players.Count == 0)
        {
            return false;
        }

        List<PlayerController> players = new List<PlayerController>();

        foreach (var player in gameManager.Players)
        {
            if (player != null)
            {
                players.Add(player);
            }
        }

        if (players.Count == 0)
        {
            return false;
        }

        // 距離順にソート
        players.Sort((a, b) =>
        {
            float distA = Vector3.Distance(transform.position, a.transform.position);
            float distB = Vector3.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB); // 昇順（近い順）
        });

        // n番目が範囲外なら一番遠いプレイヤ-
        int index = Mathf.Clamp(n - 1, 0, players.Count - 1);

        _player = players[index].gameObject;

        return _player != null;

    }
    /// <summary>
    /// 指定したインデックスのプレイヤーをターゲットにする
    /// </summary>
    /// <param name="index">n番目に近いプレイヤー </param>
    /// <returns>true プレイヤーを見つけた　false プレイヤーを見つからなかった </returns>
    protected bool FindPlayer(int index)
    {
        var players = gameManager.Players
            .Where(go => go != null) // nullチェック
            .OrderBy(go => Vector3.Distance(transform.position, go.transform.position)) // 距離順にソート
            .ToArray();

        // プレイヤーが存在しない場合は false を返す
        if (players.Length == 0) return false;

        // インデックスが範囲外の場合は false を返す
        if (index < 0 || index >= players.Length) return false;
        _player = players[index].gameObject;

        // 指定されたインデックスが要素数を超えるなら一番遠いプレイヤーを選択するパターン
        // if (index < 0) return false;
        // _player = players[Mathf.Min(index, players.Length - 1)].gameObject;

        return true;
    }

    /// <summary>
    /// 敵の移動処理
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// 敵の攻撃処理
    /// </summary>
    public virtual void Attack()
    {
        _animator.SetTrigger("Shot");
    }

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
