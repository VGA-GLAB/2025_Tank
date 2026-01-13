using CriWare;
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
    private CriAtomExPlayback _tankSePlayback;
    public HPGaugeController HPGauge;
    public BuffUIManager BuffUI;
    private bool _isDie = false;
    public void Awake()
    {
        
    }
    public void UpdateMarkerPosition()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(_playerHeadPosition.position);
        vp.y += _markerPosNormalized;
        Vector3 screenPos = Camera.main.ViewportToScreenPoint(vp);
        _playerMarker.position = screenPos;
        _playerMarker.gameObject.SetActive(photonView.IsMine);
    }

    public override void OnDisable()
    {
        if(_tankSePlayback.GetStatus() == CriAtomExPlayback.Status.Playing)
        {
            _tankSePlayback.Stop();
        }
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

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;


        if (_moveInput != Vector2.zero)
        {
            var z = _moveInput.y * _moveSpeed * Time.deltaTime;

            _rigidbody.AddForce(this.transform.forward * z, ForceMode.Impulse);
        }
    }

    private void Update()
    {

       UpdateMarkerPosition();

        if (!photonView.IsMine) return;

        if (_moveInput != Vector2.zero)
        {
            var x = _moveInput.x * _turnSpeed * Time.deltaTime;
            this.transform.Rotate(0, x, 0);

            // キャタピラの音が再生されていなかったら再生する
            if (_tankSePlayback.GetStatus() == CriAtomExPlayback.Status.Removed)
            {
                _tankSePlayback = CRIAudioManager.SE.Play("SE", "tank");
            }
        }
        else
        {
            // キャタピラの音が再生中なら停止する
            if (_tankSePlayback.GetStatus() == CriAtomExPlayback.Status.Playing)
            {
                _tankSePlayback.Stop();
            }
        }

      
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

        if (PhotonNetwork.IsMasterClient)
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
        if (photonView.IsMine)
        {
            _hp -= attack;
            if (HPGauge != null)
            {
                HPGauge.UpdateHPGauge(true);
            }
            photonView.RPC(nameof(PlayAnimation), RpcTarget.All, 0);
            if (_hp <= 0)
            {
                photonView.RPC(nameof(Die), RpcTarget.All);
                return;
            }
        }
        //PhotonView view = PhotonView.Find(viewID);
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
