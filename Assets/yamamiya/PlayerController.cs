using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Buff
{
    Hp,
    Attack,
    MoveSpeed,
    BulletInterval
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks, ITank
{
    public int Hp => _hp;
    public int AttackPower => _attackPower;
    public float MoveSpeed => _moveSpeed;

    [Header("ステータス設定")]
    [SerializeField] private int _hp; //耐久力
    [SerializeField] private int _attackPower; //攻撃力
    [SerializeField] private float _moveSpeed; //移動速度
    [SerializeField] private float _bulletInterval; //砲弾の連射インターバル
    [SerializeField] private float _turnSpeed; //回転速度

    [Header("コンポーネント")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private BulletShooter _bulletShooter;

    [Header("バフの上限設定")]
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxattackPower;
    [SerializeField] private float _maxmoveSpeed;
    [SerializeField] private float _minBulletdInterval;

    private Vector2 _moveInput;
    private GameManager gameManager;

    private void Start()
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        if (_bulletShooter == null)
        {
            _bulletShooter = GetComponent<BulletShooter>();
        }

        _bulletShooter.IntializeAttackSettings(_attackPower, _bulletInterval);
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if (_moveInput != Vector2.zero)
        {
            var x = _moveInput.x * _turnSpeed * Time.deltaTime;
            var z = _moveInput.y * _moveSpeed * Time.deltaTime;

            _rigidbody.AddForce(this.transform.forward * z, ForceMode.Impulse);

            this.transform.Rotate(0, x, 0);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    /// <summary>
    /// プレイヤーを消してリスポーン処理を実行
    /// </summary>
    public void Die()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Destroy(this.gameObject);
            gameManager.GetComponent<PhotonView>().RPC("CheckPlayerActive", RpcTarget.All, photonView.ViewID);
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
    public void OnPhotonDeastroy(PhotonMessageInfo info)
    {
        Debug.Log($"Destroyed by actor: {info.Sender}");
    }

    public void BuffStatus(Buff buff, float amount)
    {
        switch(buff)
        {
            case Buff.Hp:
                _hp += (int)amount;
                if (_hp > _maxHp)
                {
                    _hp = _maxHp;
                }
                break;
            case Buff.Attack:
                _attackPower += (int)amount;
                if (_attackPower > _maxattackPower)
                {
                    _attackPower = _maxattackPower;
                }
                break;
            case Buff.MoveSpeed:
                _moveSpeed += amount;
                if( _moveSpeed > _maxmoveSpeed)
                {
                    _moveSpeed = _maxmoveSpeed;
                }
                break;
            case Buff.BulletInterval:
                _bulletInterval -= amount;
                if(_bulletInterval < _minBulletdInterval)
                {
                    _bulletInterval = _minBulletdInterval;
                }
                break;
        }
    }
}
