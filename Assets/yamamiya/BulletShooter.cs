using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletShooter : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _bulletPrefab;//必ずResourcesフォルダにいえる
    [SerializeField] private Transform _shotPosition;
    [SerializeField] private Transform _turret;
    [SerializeField] private Animator _tankAnimator;
    public AttackIntervalGauge IntervalGauge;
    private int _attack;
    private float _bulletInterval;
    private float _intervalTimer;
    private bool _isFiring;

    private void Start()
    {
        _intervalTimer = 0;
        if(_tankAnimator == null)
        {
            _tankAnimator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (_isFiring)
        {
            ShotBullet();
        }

        if (_intervalTimer > 0f)
        {
            _intervalTimer -= Time.deltaTime;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isFiring = true;
        }
        else if (context.canceled)
        {
            _isFiring = false;
        }
    }

    /// <summary>
    /// 攻撃力と砲弾の連射インターバルを設定
    /// </summary>
    /// <param name="atk">攻撃力</param>
    /// <param name="bulletInterval">砲弾の連射インターバル</param>
    public void InitializeAttackSettings(int atk, float bulletInterval)
    {
        _attack = atk;
        _bulletInterval = bulletInterval;
    }

    /// <summary>
    /// 砲弾を発射する
    /// </summary>
    private void ShotBullet()
    {
        if (IntervalGauge == null) return;

        if (_intervalTimer <= 0f)
        {
            //Sound: 弾発射
            CRIAudioManager.SE.Play("SE", "shot");
            photonView.RPC(nameof(PlayShotAnimation), RpcTarget.All);
            _intervalTimer = _bulletInterval;
            IntervalGauge.AnimateFillAmount(1f, _bulletInterval);
            GameObject newBullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _shotPosition.position, _turret.rotation);
            newBullet.transform.forward = _turret.forward;

            if (newBullet.TryGetComponent(out BulletControl component))
            {
                component.SetBulletData(_attack, BulletControl.Target.Player,photonView);
            }
        }
    }
    [PunRPC]
    public void PlayShotAnimation()
    {
        _tankAnimator.SetTrigger("Shot");
    }
}