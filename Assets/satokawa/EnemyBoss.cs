using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using DG.Tweening;
using System;
public class EnemyBoss : EnemyBase
{
    [System.Serializable]
    public class AttackPattern
    {
        public AttackType attackType;
        public int shotCount;
        public float shotInterval;
    }
    [SerializeField, Header("攻撃パターン")]
    private List<AttackPattern> attackPatterns;
    [SerializeField, Header("散弾で左右の弾を撃つ角度")]
    private float _buckshotAngle;
    [Header("レーザー")]
    [SerializeField,Range(1,179)] private float _laserAngle;
    [SerializeField] private float _laserDistance;
    [SerializeField] private float _laserRotationSpeed;
    [SerializeField] private float _laserDamageInterval;
    [SerializeField] private LineRenderer _laserLine;

    private float _patternTimer;
    private int _patternIndex;
    private int _attackCounter;

    private float _laserTimer;
    private bool _isRaserTween = false;
    private bool _isLaser = false;
    public enum AttackType
    {
        SingleShot, Buckshot, LaserShot, Wait
    }
    protected override void Start()
    {
        base.Start();
        _patternIndex = 0;
        _patternTimer = 0;
    }
    private void Update()
    {
        if (_isLaser)
        {
            AttackRaser();
        }

        AttackPattern pattern = attackPatterns[_patternIndex];
        _patternTimer += Time.deltaTime;
        if (pattern == null || _patternTimer < pattern.shotInterval)
        {
            return;
        }

        bool isCompletedImmediately = true;

        switch (pattern.attackType)
        {
            case AttackType.SingleShot:
                Shot(this.transform.forward);
                _patternTimer = 0f;
                break;

            case AttackType.Buckshot:

                Shot(this.transform.forward);

                Quaternion rightAngle = Quaternion.Euler(0, _buckshotAngle, 0);
                Shot(rightAngle * this.transform.forward);

                Quaternion leftAngle = Quaternion.Euler(0, -_buckshotAngle, 0);
                Shot(leftAngle * this.transform.forward);

                _patternTimer = 0f;
                break;

            case AttackType.LaserShot:

                if (_isRaserTween)
                {
                    return;
                }
                isCompletedImmediately = false;

                StartLaserShotSequence(pattern);
                return;

            case AttackType.Wait:
                _patternTimer = 0f;
                break;

            default:
                Debug.LogError("未実装の攻撃方法");
                break;
        }

        //即座に完了する攻撃（SingleShot, Buckshot, Wait）のみがこの処理に進む
        if (isCompletedImmediately)
        {
            _attackCounter++;
            if (_attackCounter >= pattern.shotCount)
            {
                _attackCounter = 0;
                _patternIndex = (_patternIndex + 1) % attackPatterns.Count;
            }
        }
    }


    // AttackPatternの定義が不明なため、引数として必要な情報を渡します
    // (実際のコードに合わせて適宜修正してください)
    private void StartLaserShotSequence(AttackPattern pattern)
    {
        // DOTweenが既にアクティブでないかチェック（念のため）
        if (_isRaserTween) return;

        // 1. フラグを立て、シーケンスを開始
        _isRaserTween = true;
        Sequence sequence = DOTween.Sequence();


        // 偶数のとき +1 (右回転開始)、奇数のとき -1 (左回転開始)
        int powerResult = (int)Math.Pow(-1, _attackCounter); 
        float startAngle = -powerResult * _laserAngle /2;      
        float endAngle = powerResult * _laserAngle /2 ;      

        // ----------------------------------------------------
        // シーケンスの定義
        // ----------------------------------------------------

        // 1. ターレットを指定角度へ回転 (首振り開始)
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(0, startAngle, 0),
            pattern.shotInterval / 2
        ));

        // 2. レーザー発射開始
        sequence.AppendCallback(() =>
        {
            _laserLine.enabled = true;
            _isLaser = true;
            _laserTimer = _laserDamageInterval;
        });

        // 3. レーザーを出しながら反対側へ回転 (レーダー発射)
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(0, endAngle, 0),
            _laserRotationSpeed
        ));

        // 4. レーザー発射終了
        sequence.AppendCallback(() =>
        {
            _laserLine.enabled = false;
            _isLaser = false;
        });

        // 5. 元の角度（0度）に戻す
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(0, 0, 0),
            pattern.shotInterval / 2
        ));

        // 6. シーケンス完了時の処理 (タイマー/カウンタの更新)
        sequence.AppendCallback(() =>
        {
            _isRaserTween = false;
            _patternTimer = 0f;

            _attackCounter++;
            if (_attackCounter >= pattern.shotCount)
            {
                _attackCounter = 0;
                _patternIndex = (_patternIndex + 1) % attackPatterns.Count;
            }
        });

        // シーケンスを再生
        sequence.Play();
    }
    private void AttackRaser()
    {
        Ray ray = new Ray(_muzzlePosition.position, _muzzlePosition.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin,ray.direction, _laserDistance);
        Array.Sort(hits,(a,b) => a.distance.CompareTo(b.distance));
        _laserTimer += Time.deltaTime;
        float stopDistance = _laserDistance;

        foreach(RaycastHit hit in hits)
        {
            if (!hit.collider.TryGetComponent(out ITank tank) && !hit.collider.TryGetComponent(out ItemBase item))
            {//遮蔽物に当たった
                stopDistance = hit.distance;
                break;
            }
            if (tank != null && _laserTimer > _laserDamageInterval)
            {
                hit.collider.GetComponent<PhotonView>().RPC("Hit", RpcTarget.All, _attack);
                _laserTimer = 0;
            }
        }
        _laserLine.SetPosition(0,ray.origin);
        _laserLine.SetPosition(1, ray.origin + ray.direction * stopDistance);

    }
    public override void Attack()
    {

    }
    private void Shot(Vector3 direction)
    {
        if (photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            GameObject bullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _muzzlePosition.position, Quaternion.LookRotation(direction));
        }
    }
    public override void Move()
    {

    }
}