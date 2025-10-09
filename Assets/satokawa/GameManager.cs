using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("次のステージ（Scene）")] private string _nextScene;
    [SerializeField, Header("リスポーン時間")] private float _respawnTime;

    private List<PlayerController> _players = new List<PlayerController>(); 
    private List<EnemyBase> _enemys = new List<EnemyBase>(); 

    private bool _isRespawnTimer = false;
    private float _timer;
    private InGameNetworkManager _networkManager;
    private void Start()
    {
        _networkManager = GetComponent<InGameNetworkManager>();
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
    ///[PunRPC] NetWorkMagagerで生成したプレイヤーを保存する
    /// </summary>
    /// <<param name="newPlayerViewID">photonViewのviewIDを入れる</param>>
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
    /// <summary>
    /// NetWorkMagagerで生成した敵を保存する
    /// </summary>
    /// <param name="newEnemyViewID">photonViewのviewIDを入れる</param>
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
    /// <summary>
    ///[PunRPC]　プレイヤーのHPを確認して必要に応じてゲームオーバーやリスポーン処理を実行
    /// </summary>
    /// <param name="diePlayerID">photonViewのviewIDを入れる</param>
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
    /// <summary>
    /// photonView.viewIDをPlayerController変換
    /// </summary>
    /// <param name="diePlayerID">photonViewのviewIDを入れる</param>
    /// <returns>photonView.viewIDをPlayerController変換した値</returns>
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
    /// <summary>
    ///[PunRPC] 敵のHPを確認してすべて倒していたらゲームオーバーを実行
    /// </summary>
    [PunRPC]
    public void CheckEnemeyActive()
    {
        //マスターのみ実行
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

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
    ///[PunRPC] ゲームオーバー処理
    /// 現在のステージをリロードする
    /// </summary>
    [PunRPC]
    private void GameOver()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    ///[PunRPC] ゲームクリア処理　
    /// </summary>
    [PunRPC]
    private void GameClear()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(_nextScene);
    }
}