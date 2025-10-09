using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("プレイヤー")] private List<PlayerController> _players; 
    [SerializeField, Header("敵")] private List<EnemyBase> _enemys; 
    [SerializeField, Header("次のステージ（Scene）")] private string _nextScene;
    [SerializeField, Header("リスポーン時間")] private float _respawnTime;
    private bool _isRespawnTimer = false;
    private float _timer;
    private NetworkManager _networkManager;
    private void Start()
    {
        _networkManager = GetComponent<NetworkManager>();
        _players.Clear();
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
    [PunRPC]
    public void AddPlayer(int newPlayerViewID)
    {
        PlayerController newPlayer = GetPlayerController(newPlayerViewID);
        if (newPlayer == null)
        {
            return;
        }
            _players.Add(newPlayer);
    }
    [PunRPC]
    public void AddEnemy(int newEnemyViewID)
    {
        PhotonView targetView = PhotonView.Find(newEnemyViewID);
        if (targetView == null)
        {
            return;
        }
        EnemyBase newEnemy = targetView.GetComponent<EnemyBase>();
        if (newEnemy == null)
        {
            return;
        }
        _enemys.Add(newEnemy);
    }
    [PunRPC]
    public void CheckPlayerActive(int  diePlayerID)
    {
        PlayerController diePlayer = GetPlayerController(diePlayerID);
        if( diePlayer == null)
        {
            return;
        }

        bool isPlayerActive = false;
        foreach (PlayerController tank in _players)
        {
            if (tank != null && tank.Hp > 0)
            {
                isPlayerActive = true;
            }
        }
        if (!isPlayerActive && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(GameOver), RpcTarget.All);
        }
        else if (diePlayer.GetComponent<PhotonView>().IsMine)
        {
            _isRespawnTimer = true;
            _timer = 0;
        }
    }

    private PlayerController GetPlayerController(int diePlayerID)
    {
        PhotonView targetView = PhotonView.Find(diePlayerID);
        if (targetView == null)
        {
            return null;
        }
        PlayerController player = targetView.GetComponent<PlayerController>();
        return player;
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
        if (!isEnemyActive && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(GameClear), RpcTarget.All);
        }
    }


    /// <summary>
    /// ゲームオーバー処理
    /// 現在のステージをリロードする
    /// </summary>
    [PunRPC]
    private void GameOver()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    [PunRPC]
    private void GameClear()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(_nextScene);
    }
}