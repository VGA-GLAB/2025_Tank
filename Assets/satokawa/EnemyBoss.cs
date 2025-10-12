using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;
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
    [SerializeField,Header("散弾で左右の弾を撃つ角度")] 
    private float _buckshotAngle;
    private float timer;
    private int patternIndex;
    private int attackCounter;
    
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

        switch (pattern.attackType)
        {
            case AttackType.SingleShot:
                Shot(this.transform.forward);
                break;
            case AttackType.Buckshot:

                // 真ん中
                Shot(this.transform.forward);

                // 右側
                Quaternion rightAngle = Quaternion.Euler(0, _buckshotAngle, 0);
                Shot(rightAngle * this.transform.forward);

                //左側
                Quaternion leftAngle = Quaternion.Euler(0, -_buckshotAngle, 0);
                Shot(leftAngle * this.transform.forward);

                break;
            case AttackType.LaserShot:
                break;
            case AttackType.Wait:
                break;
            default:
                Debug.LogError("未実装の攻撃方法");
                break;
        }
        timer = 0f;


        if(attackCounter >= pattern.shotCount)
        {
            attackCounter = 0;
            patternIndex = (patternIndex + 1) % attackPatterns.Count;
        }
        else
        {
            attackCounter++;
        }
    }
    public override void Attack()
    {

    }
    private void Shot(Vector3 direction)
    {
        if(photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            GameObject bullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _muzzlePosition.position, Quaternion.LookRotation(direction));
        }
    }
    private void Laser()
    {

    }
    public override void Move()
    {

    }
}