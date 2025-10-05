using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ITank player; //プレイヤー　あとでプレイヤークラスに変える
    [SerializeField] private ITank[] enemys; //敵　あとで書き換え
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (player.Hp <= 0)
            GameOver();

        bool isTankActive = false;
        foreach (ITank tank in enemys)
        {
            //Move処理

            if (tank != null && tank.Hp > 0)
            { 
                isTankActive =true;
            }
        }
        if (!isTankActive)
        {
            GameClear();
        }
    }
    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    private void GameOver()
    {
        Debug.Log("ゲームオーバー");
    }
    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    private void GameClear()
    {
        Debug.Log("ゲームクリア");
    }
}
