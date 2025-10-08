using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField, Header("プレイヤー")] private List<PlayerController> _players; 
    [SerializeField, Header("敵")] private EnemyBase[] _enemys; 
    [SerializeField, Header("次のステージ（Scene）")] private string _nextScene;
    [SerializeField, Header("リスポーン時間")] private float _respawnTime;
    private bool _isRespawnTimer = false;
    private float _timer;
    private NetworkManager _networkManager;
    private void Start()
    {
        _networkManager = GetComponent<NetworkManager>();
    }
    public void Update()
    {
        //リスポーンタイマーを動かし時間になったらNetworkmanagerにPlayerを作ってもらう
        if (_isRespawnTimer)
        {
            _timer += Time.time;
            if(_timer > _respawnTime)
            {
                _networkManager.CreatePlayerTank();
                _isRespawnTimer = false;
            }
        }    
    }
    /// <summary>
    /// プレイヤーをGameManagerに渡してHPを確認してもらう
    /// </summary>
    /// <param name="newPlayer">NetworkManagerで生成したプレイヤー</param>
    public void AddPlayer(PlayerController newPlayer)
    {
        _players.Add(newPlayer);
    }
    [PunRPC]
    public void CheckPlayerActive(PlayerController diePlayer)
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
        else if (diePlayer.GetComponent<PhotonView>().IsMine)
        {
            _isRespawnTimer = true;
            _timer = 0;
        }
    }
　　[PunRPC]
    public void CheckEnemeyActive()
    {
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