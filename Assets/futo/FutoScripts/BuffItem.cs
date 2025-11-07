using Photon.Pun;
using UnityEngine;

public class BuffItem : ItemBase
{
    [SerializeField] private  Buff _toBuff;
    [SerializeField] private float _buffAmount;
    [SerializeField] private ParticleSystem _buffParticle;

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
            GameObject newParticle =  Instantiate(_buffParticle.gameObject,target.transform);
            newParticle.transform.localPosition = Vector3.zero;
            newParticle.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }
        else
        {
            return;
        }
        Delete();
    }
}