using Photon.Pun;
using UnityEngine;

public class BuffItem : ItemBase
{
    [SerializeField] private  Buff _toBuff;
    [SerializeField] private float _buffAmount;
    [SerializeField] private ParticleSystem _buffParticle;
    [SerializeField] private Color _particleColor;

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
            if(target.GetComponent<PhotonView>().IsMine)
            {
            CRIAudioManager.SE.Play("SE", "itemget");
            }
            target.BuffStatus(_toBuff, _buffAmount);
            GameObject newParticle =  Instantiate(_buffParticle.gameObject,target.transform);
            newParticle.transform.localPosition = Vector3.zero;
            newParticle.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            ParticleSystem.MainModule main = newParticle.GetComponent<ParticleSystem>().main;
            ParticleSystem.MainModule sub = newParticle.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            main.startColor = _particleColor;
            sub.startColor = _particleColor;
        }
        else
        {
            return;
        }
        Delete();
    }
}