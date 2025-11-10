using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
public class TankUIControl : MonoBehaviour
{
    [SerializeField] private RawImage[] _tankImage;
    [SerializeField] private Material[] _tankMaterial;
    [SerializeField] private Material _hiddenMaterial;
    [SerializeField] private Button _startButton;
    public void JoinNewPlayer()
    {
        int i = 1;
        int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        foreach (RawImage image in _tankImage)
        {
            if (i <= playerNumber)
            {
                image.color = Color.white;
            }
            else
            {
                image.color = Color.black;
            }
            i++;
        }

        _startButton.interactable = PhotonNetwork.IsMasterClient;
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