using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
public class TankUIControl : MonoBehaviour
{
    [SerializeField] private RawImage[] _tankImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void JoinNewPlayer()
    {
        int i = 1;
        int playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        foreach (RawImage image in _tankImage)
        {
            image.gameObject.SetActive(i <= playerNumber);
            i++;
        }
    }
}
