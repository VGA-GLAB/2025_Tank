using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject roomJoin;
    [SerializeField] private GameObject stageSelectSingle;
    [SerializeField] private GameObject stageSelectMulti;
    private void Start()
    {
        ChangeScreen(0);
    }
        
    public void ChangeScreen(int number)
    {
        title.SetActive(number == 0);
        roomJoin.SetActive(number == 1);
        stageSelectSingle.SetActive(number == 2);
        stageSelectMulti.SetActive(number == 3);
    }
}
