using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
public class TankUIControl : MonoBehaviour
{
    [SerializeField] private GameObject[] _tankObject;
    [SerializeField] private Material[] _tankMaterial;
    [SerializeField] private Material _hiddenMaterial;

    public void JoinNewPlayer()
    {
        int i = 1;
        int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        foreach (GameObject tank in _tankObject)
        {
            if (i <= playerNumber)
            {
                ChangeMaterial(_tankMaterial[i - 1], tank);
            }
            else
            {
                ChangeMaterial(_hiddenMaterial, tank);
            }
            i++;
        }
    }
    private void ChangeMaterial(Material material, GameObject tank)
    {
        Transform[] allChildren = tank.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            if (child == tank) continue; // 親自身を除外したい場合
            if (child.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = material;
            }
        }
    }
}