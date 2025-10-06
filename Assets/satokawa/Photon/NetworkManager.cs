using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
/// <summary>
/// PhotonのNetworkを管理
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField,Header("プレイヤーPrefab")] private GameObject tankPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }
    /// <summary>
    /// プレイヤー生成処理
    /// </summary>
    public override void OnJoinedRoom()
    {
        Vector3 position = new Vector3(Random.Range(-3f, 3f), 3f, Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("Tank", position, Quaternion.identity);
    }
}