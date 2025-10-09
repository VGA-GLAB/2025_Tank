using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// タイトルのネットワークを管理
/// </summary>
public class TitleNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button joinButton;
    [SerializeField] private Button startButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        joinButton.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        startButton.interactable = PhotonNetwork.InRoom;
    }
    public void JoinMaster()
    {
        joinButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        
    }
    public void GameStart()
    {
        photonView.RPC(nameof(LoadInGameScene), RpcTarget.All);
    }
    [PunRPC]
    public void LoadInGameScene()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("Stage1");
    }
}
