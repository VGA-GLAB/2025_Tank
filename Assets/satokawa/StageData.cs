using UnityEngine;

[CreateAssetMenu(fileName = "StageData")]
public class StageData : ScriptableObject
{
    public string Number;
    public string Name;
    public Sprite Image;
    [Header("敵の数")]
    public int NormalEnemy;//Enemy
    public int LaserEnemy;//Laser
    public int BuckshotEnemy;//ショットガン
    public int FlankingEnemy;//背後
    public int FixedEnemy;//固定
    public int BossEnemy;//ボス


}
