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
            image.transform.GetChild(0).gameObject.SetActive(i == PhotonNetwork.LocalPlayer.ActorNumber);
            i++;
        }

        _startButton.interactable = PhotonNetwork.IsMasterClient;
    }
}