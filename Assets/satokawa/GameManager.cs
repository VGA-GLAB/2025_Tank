using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField,Header("プレイヤー")] private ITank[] player; //TODO: 型をPlayerに変える
    [SerializeField, Header("敵")] private ITank[] enemys; //TODO：　型をEnemyに変える
    void Update()
    {
        bool isPlayerActive = false;
        foreach (ITank tank in enemys)
        {
            if (tank != null && tank.Hp > 0)
            {
                isPlayerActive = true;
            }
        }
        if(!isPlayerActive)
        {
            GameOver();
        }

        bool isTankActive = false;
        foreach (ITank tank in enemys)
        {
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
