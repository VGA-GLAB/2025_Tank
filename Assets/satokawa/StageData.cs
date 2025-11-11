using UnityEngine;

[CreateAssetMenu(fileName = "StageData")]
public class StageData : ScriptableObject
{
    public string Number;
    public string Name;
    public Sprite Image;
    [Header("敵の数")]
    public int NormalEnemy;
    public int LaserEnemy;
    public int BuckshotEnemy;
    public int FlankingEnemy;
    public int BossEnemy;


}
