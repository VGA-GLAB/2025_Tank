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
    [SerializeField] private float _markerPosNormalized = 0.05f;

    [Header("コンポーネント")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private BulletShooter _bulletShooter;
    [SerializeField] private Animator _tankAnimator;
    [SerializeField] private Animator _hamsterAnimator;
    [SerializeField] private ParticleSystem _killEffect;
    [SerializeField] private RectTransform _playerMarker;
    [SerializeField] private Transform _playerHeadPosition;
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
    private bool _isDie = false;
    public void Awake()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(_playerHeadPosition.position);
        vp.y += _markerPosNormalized;
        Vector3 screenPos = Camera.main.ViewportToScreenPoint(vp);
        _playerMarker.position = screenPos;
        _playerMarker.gameObject.SetActive(photonView.IsMine);
    }
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

        _isDie = false;
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


        Vector3 vp = Camera.main.WorldToViewportPoint(_playerHeadPosition.position);
        vp.y += _markerPosNormalized;
        Vector3 screenPos = Camera.main.ViewportToScreenPoint(vp);
        _playerMarker.position = screenPos;


    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    /// <summary>
    /// プレイヤーを消してリスポーン処理を実行
    /// </summary>
    [PunRPC]
    public void Die()
    {
        if (_isDie) return;

        _isDie = true;

        if (photonView.IsMine)
        {
            photonView.RPC(nameof(PlayAnimation), RpcTarget.All, 1);
        }
        DOVirtual.DelayedCall(1f, () =>
        {
            Instantiate(_killEffect, this.transform.position, Quaternion.identity);
            if (photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
            {
                CRIAudioManager.SE.Play("SE", "kill");
                _gameManager.GetComponent<PhotonView>().RPC("CheckPlayerActive", RpcTarget.All, photonView.ViewID);
            }
        });
    }
    [PunRPC]
    public void Hit(int attack, int viewID)
    {
        _hp -= attack;
        if (photonView.IsMine && HPGauge != null)
        {
            HPGauge.UpdateHPGauge(true);
            photonView.RPC(nameof(PlayAnimation), RpcTarget.All, 0);
            CRIAudioManager.SE.Play("SE", "hit");
        }
        if (_hp <= 0 && (PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode))
        {
            photonView.RPC(nameof(Die), RpcTarget.All);
            return;
        
        }
    }
    [PunRPC]
    public void PlayAnimation(int animation)
    {
        switch (animation)
        {
            case 0:
                _hamsterAnimator.SetTrigger("Hit");
                break;
            case 1:
                _tankAnimator.SetTrigger("Dead");
                _hamsterAnimator.SetTrigger("Dead");
                break;
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
        HPGauge.SetTarget(this.gameObject);
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
                    HPGauge.UpdateHPGauge(true);
                }
                break;
            case Buff.Attack:
                _attackPower += (int)amount;
                if (_attackPower > _maxAttackPower)
                {
                    _attackPower = _maxAttackPower;
                }

                _bulletShooter.InitializeAttackSettings(_attackPower, _bulletInterval);
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
                _bulletShooter.InitializeAttackSettings(_attackPower, _bulletInterval);
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