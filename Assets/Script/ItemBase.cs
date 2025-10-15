using UnityEngine;
using Photon.Pun;
/// <summary>
/// アイテムの基底クラス
/// </summary>
public abstract class ItemBase : MonoBehaviourPunCallbacks
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && photonView.IsMine && PhotonNetwork.AutomaticallySyncScene)
        {
            if (other.TryGetComponent(out PhotonView view))
            {
                photonView.RPC(nameof(HitAction), RpcTarget.All, view.ViewID); // アイテム取得時の処理
            }
            else
            {
                Debug.LogError("PhotonViewがアタッチされていません");
            }
        }
    }

    /// <summary>
    /// アイテム取得時の処理
    /// </summary>
    /// <param name="viewID">アイテムを取得したオブジェクトのviewID</param>
    [PunRPC]
    public abstract void HitAction(int viewID);

    /// <summary>
    /// HitAction実行後にアイテムを消す
    /// </summary>
    public void Delete()
    {
        if (photonView.IsMine && PhotonNetwork.AutomaticallySyncScene)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
