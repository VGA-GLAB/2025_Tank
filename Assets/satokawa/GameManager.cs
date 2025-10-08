using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField, Header("プレイヤー")] private List<PlayerController> _players; 
    [SerializeField, Header("敵")] private EnemyBase[] _enemys; 
    [SerializeField, Header("次のステージ（Scene）")] private string _nextScene;

    /// <summary>
    /// プレイヤーをGameManagerに渡してHPを確認してもらう
    /// </summary>
    /// <param name="newPlayer">NetworkManagerで生成したプレイヤー</param>
    public void AddPlayer(PlayerController newPlayer)
    {
        _players.Add(newPlayer);
    }
    public void CheckTankActive()
    {
        bool isPlayerActive = false;
        foreach (PlayerController tank in _players)
        {
            if (tank != null && tank.Hp > 0)
            {
                isPlayerActive = true;
            }
        }
        if (!isPlayerActive)
        {
            GameOver();
        }

        bool isEnemyActive = false;
        foreach (EnemyBase tank in _enemys)
        {
            if (tank != null && tank.Hp > 0)
            {
                isEnemyActive = true;
            }
        }
        if (!isEnemyActive)
        {
            GameClear();
        }
    }

    /// <summary>
    /// ゲームオーバー処理
    /// 現在のステージをリロードする
    /// </summary>
    private void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("ゲームオーバー");
    }
    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    private void GameClear()
    {
        SceneManager.LoadScene(_nextScene);
        Debug.Log("ゲームクリア");
    }
}