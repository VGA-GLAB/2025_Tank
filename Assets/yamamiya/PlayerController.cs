using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private Animator _tankAnimator;
    [SerializeField] private Animator _hamsterAnimator;
    [SerializeField] private ParticleSystem _killEffect;
    [SerializeField] private RectTransform _playerMarker;
    [Header("バフの上限設定")]
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxAttackPower;
    [SerializeField] private float _maxMoveSpeed;
    [SerializeField] private float _minBulletdInterval;

    private Vector2 _moveInput;
    private GameManager _gameManager;
    private InGameNetworkManager _inGameNetworkManager;
    public HPGaugeController HPGauge;
    public BuffUIManager BuffUI;
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
        if (_tankAnimator == null)
        {
            _tankAnimator = GetComponent<Animator>();
        }

        _bulletShooter.InitializeAttackSettings(_attackPower, _bulletInterval);
        _gameManager = FindAnyObjectByType<GameManager>();
        _inGameNetworkManager = FindAnyObjectByType<InGameNetworkManager>();

        if (!photonView.IsMine)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                if (this.transform.GetChild(i).TryGetComponent(out SkinnedMeshRenderer renderer))
                {
                    renderer.material = _inGameNetworkManager._playerMaterials[NetworkCore.GetPlayerNumber(photonView.Owner) - 1];
                }
            }
        }
        _playerMarker.gameObject.SetActive(photonView.IsMine);
    }

    private void Update()
    {
        if (_moveInput != Vector2.zero)
        {
            var x = _moveInput.x * _turnSpeed * Time.deltaTime;
            var z = _moveInput.y * _moveSpeed * Time.deltaTime;

            _rigidbody.AddForce(this.transform.forward * z, ForceMode.Impulse);

            this.transform.Rotate(0, x, 0);
            //Sound:キャタピラ
        }

        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.x = this.transform.position.x;
        _playerMarker.transform.LookAt(cameraPosition);
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
            //Sound:Player DieSE （当たった瞬間はここ）
            _tankAnimator.SetTrigger("Dead");
            _hamsterAnimator.SetTrigger("Dead");
            DOVirtual.DelayedCall(1f, () =>
            {
                //Sound:Player DieSE （消える時はここ）
                CRIAudioManager.SE.Play("SE", "kill");
                Instantiate(_killEffect, this.transform.position, Quaternion.identity);
                _gameManager.GetComponent<PhotonView>().RPC("CheckPlayerActive", RpcTarget.All, photonView.ViewID);
            });
        }
    }
    [PunRPC]
    public void Hit(int attack, int viewID)
    {
        _hp -= attack;
        if (photonView.IsMine && HPGauge != null)
        {
            HPGauge.UpdateHPGauge(true);
        }
        if (_hp <= 0)
        {
            Die();
        }
        else
        {
            _hamsterAnimator.SetTrigger("Hit");
            CRIAudioManager.SE.Play("SE", "hit");
        }

    }
    public void OnPhotonDeastroy(PhotonMessageInfo info)
    {
        Debug.Log($"Destroyed by actor: {info.Sender}");
    }

    public void SetHP(int value)
    {
        _hp = value;
        _maxHp = value;
    }

    public void BuffStatus(Buff buff, float amount)
    {
        switch (buff)
        {
            case Buff.Hp:
                _hp += (int)amount;
                if (_hp > _maxHp)
                {
                    _hp = _maxHp;

                }
                if (photonView.IsMine && HPGauge != null)
                {
                    HPGauge.UpdateHPGauge();
                }
                break;
            case Buff.Attack:
                _attackPower += (int)amount;
                if (_attackPower > _maxAttackPower)
                {
                    _attackPower = _maxAttackPower;
                }
                break;
            case Buff.MoveSpeed:
                _moveSpeed += amount;
                if (_moveSpeed > _maxMoveSpeed)
                {
                    _moveSpeed = _maxMoveSpeed;
                }
                break;
            case Buff.BulletInterval:
                _bulletInterval -= amount;
                if (_bulletInterval < _minBulletdInterval)
                {
                    _bulletInterval = _minBulletdInterval;
                }
                break;
            default:
                return;
        }
        if (photonView.IsMine)
        {
            BuffUI.AddBuff(buff);
        }
    }
}