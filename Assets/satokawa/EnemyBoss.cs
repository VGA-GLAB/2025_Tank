using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System;
using static EnemyBoss;
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

    [SerializeField, Header("レーダー角度"),Range(1,179)]
    private float _radarAngle;
    [SerializeField, Header("レーダー距離")]
    private float _radarDistance;
    [SerializeField, Header("レーダースピード")]
    private float _radarSpeed;

    private float timer;
    [SerializeField] private int patternIndex;
    [SerializeField] private int attackCounter;
    private bool isRaserTween = false;
    public enum AttackType
    {
        SingleShot, Buckshot, LaserShot, Wait
    }
    private void Start()
    {
        patternIndex = 0;
        timer = 0;
    }
    private void Update()
    {

        AttackPattern pattern = attackPatterns[patternIndex];
        timer += Time.deltaTime;
        if (pattern == null || timer < pattern.shotInterval)
        {
            return;
        }

        bool isCompletedImmediately = true;

        switch (pattern.attackType)
        {
            case AttackType.SingleShot:
                Shot(this.transform.forward);
                timer = 0f;
                break;

            case AttackType.Buckshot:

                Shot(this.transform.forward);

                Quaternion rightAngle = Quaternion.Euler(0, _buckshotAngle, 0);
                Shot(rightAngle * this.transform.forward);

                Quaternion leftAngle = Quaternion.Euler(0, -_buckshotAngle, 0);
                Shot(leftAngle * this.transform.forward);

                timer = 0f;
                break;

            case AttackType.LaserShot:

                if (isRaserTween)
                {
                    return;
                }
                isCompletedImmediately = false;

                StartLaserShotSequence(pattern);
                return;

            case AttackType.Wait:
                timer = 0f;
                break;

            default:
                Debug.LogError("未実装の攻撃方法");
                break;
        }

        //即座に完了する攻撃（SingleShot, Buckshot, Wait）のみがこの処理に進む
        if (isCompletedImmediately)
        {
            attackCounter++;
            if (attackCounter >= pattern.shotCount)
            {
                attackCounter = 0;
                patternIndex = (patternIndex + 1) % attackPatterns.Count;
            }
        }
    }


    // AttackPatternの定義が不明なため、引数として必要な情報を渡します
    // (実際のコードに合わせて適宜修正してください)
    private void StartLaserShotSequence(AttackPattern pattern)
    {
        // DOTweenが既にアクティブでないかチェック（念のため）
        if (isRaserTween) return;

        // 1. フラグを立て、シーケンスを開始
        isRaserTween = true;
        Sequence sequence = DOTween.Sequence();

        // 攻撃回数に応じて回転の方向を決定する係数
        // 偶数のとき -1 (左回転開始)、奇数のとき +1 (右回転開始) にしたい場合:
        // int directionCoefficient = (int)Math.Pow(-1, attackCounter) * -1; // 偶数:-1, 奇数:1

        // 提示されたコードに合わせて、偶数のとき +1 (右回転開始)、奇数のとき -1 (左回転開始)
        // 提示されたコード: DOLocalRotate(new Vector3(0, -(int)Math.Pow(-1, attackCounter) * _radarAngle, 0), ...)
        int powerResult = (int)Math.Pow(-1, attackCounter); // 偶数: +1, 奇数: -1
        float startAngle = -powerResult * _radarAngle /2;      // 偶数: -_radarAngle, 奇数: +_radarAngle
        float endAngle = powerResult * _radarAngle /2 ;         // 偶数: +_radarAngle, 奇数: -_radarAngle

        // ----------------------------------------------------
        // 2. シーケンスの定義
        // ----------------------------------------------------

        // 1. ターレットを指定角度へ回転 (首振り開始)
        // 提示コードではDOLocalRotateを使っていたため、そちらを使用
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(0, startAngle, 0),
            pattern.shotInterval / 2
        ));

        // 2. レーザー発射開始
        sequence.AppendCallback(() =>
        {
            RaserStart();
        });

        // 3. レーザーを出しながら反対側へ回転 (レーダー発射)
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(0, endAngle, 0),
            _radarSpeed // <-- ここはレーザー発射にかける時間として設定
        ));

        // 4. レーザー発射終了
        sequence.AppendCallback(() =>
        {
            RaserEnd();
        });

        // 5. 元の角度（0度）に戻す
        sequence.Append(_turret.transform.DOLocalRotate(
            new Vector3(0, 0, 0),
            pattern.shotInterval / 2
        ));

        // 6. シーケンス完了時の処理 (タイマー/カウンタの更新)
        sequence.AppendCallback(() =>
        {
            // 制御フラグを解除
            isRaserTween = false;

            // タイマーをリセット
            timer = 0f;

            // カウンタを進め、次のパターンへ遷移
            attackCounter++;
            if (attackCounter >= pattern.shotCount)
            {
                attackCounter = 0;
                patternIndex = (patternIndex + 1) % attackPatterns.Count;
            }
        });

        // シーケンスを再生
        sequence.Play();
    }
    private void RaserStart()
    {

    }
    private void RaserEnd()
    {

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