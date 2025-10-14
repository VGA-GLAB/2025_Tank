using UnityEngine;
using Photon.Pun;
/// <summary>
/// アイテムの基底クラス
/// </summary>
public abstract class ItemBase : MonoBehaviourPunCallbacks
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && photonView.IsMine && PhotonNetwork.AutomaticallySyncScene)
        {
           photonView.RPC(nameof(HitAction), RpcTarget.All,other.gameObject); // アイテム取得時の処理
        }
    }

    /// <summary>
    /// アイテム取得時の処理
    /// </summary>
    /// <param name="hitObject">アイテムを取得したオブジェクト</param>
    [PunRPC]
    public abstract void HitAction(GameObject hitObject);

    /// <summary>
    /// HitAction実行後にアイテムを消す
    /// </summary>
    public void Delete()
    {
        if(photonView.IsMine && PhotonNetwork.AutomaticallySyncScene)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
