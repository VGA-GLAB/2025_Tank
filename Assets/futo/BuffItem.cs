using Photon.Pun;
using UnityEngine;

public class BuffItem : ItemBase
{
    [SerializeField] Buff _toBuff;
    [SerializeField] float _buffAmount;

    [PunRPC]
    public override void HitAction(int viewID)
    {
        GameObject hitObject = PhotonView.Find(viewID).gameObject;
        if (hitObject == null)
        {
            Debug.LogError("HitAction: hitObject is null");
            return;
        }
        if (hitObject.TryGetComponent(out PlayerController target))
        {
            target.BuffStatus(_toBuff, _buffAmount);
        }
        else
        {
            return;
        }
        Delete();
    }
}