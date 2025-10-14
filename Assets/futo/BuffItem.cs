using UnityEngine;
using Photon.Pun;
public class BuffItem : ItemBase
{
    [SerializeField] Buff _toBuff;
    [SerializeField] float _buffAmount;

    [PunRPC]
    public override void HitAction(GameObject hitObject)
    {
        if(hitObject.TryGetComponent(out PlayerController target))
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
