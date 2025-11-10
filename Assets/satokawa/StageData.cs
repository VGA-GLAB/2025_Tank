using UnityEngine;

[CreateAssetMenu(fileName = "StageData")]
public class StageData : ScriptableObject
{
    public string _number;
    public string _name;
    public Sprite _image;
    [Header("敵の数")]
    public int _normalEnemy;
    public int _laserEnemy;
    public int _buckshotEnemy;
    public int _flankingEnemy;
    public int _bossEnemy;


}
