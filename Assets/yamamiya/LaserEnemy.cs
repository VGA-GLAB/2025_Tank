using DG.Tweening;
using Photon.Pun;
using System;
using UnityEngine;

public class LaserEnemy : EnemyBase
{
    [Header("攻撃設定")]
    [SerializeField, Tooltip("攻撃する間隔")] private float _attackInterval;

    [Header("レーザー")]
    [SerializeField, Range(1, 179)] private float _laserAngle;
    [SerializeField] private float _laserDistance;
    [SerializeField] private float _laserRotationSpeed;
    [SerializeField] private float _laserDamageInterval;
    [SerializeField] private LineRenderer _laserLine;

    private int _attackCounter = 1;
    private float _attackTimer;
    private float _laserTimer;
    private bool _isRaserTween = false;
    private bool _isLaser = false;

    protected override void Start()
    {
        base.Start();
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(SetLaserLineEnabled), RpcTarget.All,0);
        }
    }

    public void Update()
    {
        Attack();

        if (!photonView.IsMine)
        {
            return;
        }

        if (!PlayerFind())
        {
            return;
        }

        if (_isRaserTween)
        {
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _attackInterval)
        {
            StartLaserShotSequence();
        }
    }

    /// <summary>
    /// レーザー攻撃のシーケンスを開始
    /// </summary>
    private void StartLaserShotSequence()   
    {
        if (_isRaserTween) return;
        _isRaserTween = true;
        Sequence sequence = DOTween.Sequence();

        _attackCounter = UnityEngine.Random.value > 0.5f ? 1 : 2;
        // 偶数のとき +1 (右回転開始)、奇数のとき -1 (左回転開始)
        int powerResult = (int)Math.Pow(-1, _attackCounter);
        float startAngle = -powerResult * _laserAngle / 2;
        float endAngle = powerResult * _laserAngle / 2;

        // ----------------------------------------------------
        // シーケンスの定義
        // ----------------------------------------------------

        // 1. ターレットを指定角度へ回転 (首振り開始)
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(startAngle,0, 0),
            _attackInterval / 2
        ));

        // 2. レーザー発射開始
        sequence.AppendCallback(() =>
        {
            photonView.RPC(nameof(SetLaserLineEnabled), RpcTarget.All,1);
            _isLaser = true;
            _laserTimer = _laserDamageInterval;
        });

        // 3. レーザーを出しながら反対側へ回転 (レーダー発射)
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(endAngle,0 , 0),
            _laserRotationSpeed
        ));

        // 4. レーザー発射終了
        sequence.AppendCallback(() =>
        {
            photonView.RPC(nameof(SetLaserLineEnabled), RpcTarget.All,0);
            _laserLine.enabled = false;
            _isLaser = false;
        });

        // 5. 元の角度（0度）に戻す
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(0, 0, 0),
            _attackInterval / 2
        ));

        // 6. シーケンス完了時の処理 (タイマー/カウンタの更新)
        sequence.AppendCallback(() =>
        {
            _isRaserTween = false;
            _attackTimer = 0;
        });

        // シーケンスを再生
        sequence.Play();
    }
    [PunRPC]
    private void SetLaserLineEnabled(int b)
    {
        _laserLine.enabled = b == 1 ;
    }

    /// <summary>
    /// レーザー攻撃
    /// </summary>
    public override void Attack()
    {
        if(!_laserLine.enabled) return;

        Ray ray = new Ray(_muzzlePosition.position, _muzzlePosition.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, _laserDistance);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        _laserTimer += Time.deltaTime;
        float stopDistance = _laserDistance;

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.TryGetComponent(out ITank tank) && !hit.collider.TryGetComponent(out ItemBase item))
            {//遮蔽物に当たった
                stopDistance = hit.distance;
                break;
            }
            if (hit.collider.TryGetComponent(out EnemyBase enemy))
            {
                continue;
            }
            if (tank != null && _laserTimer > _laserDamageInterval)
            {
                hit.collider.GetComponent<PhotonView>().RPC("Hit", RpcTarget.All, _attack,photonView.ViewID);
                _laserTimer = 0;
            }
        }
        _laserLine.SetPosition(0, ray.origin);
        _laserLine.SetPosition(1, ray.origin + ray.direction * stopDistance);
    }

    public override void Move() { }
}
