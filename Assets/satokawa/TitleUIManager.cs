using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _title;
    [SerializeField] private GameObject _roomJoin;
    [SerializeField] private GameObject _stageSelectSingle;
    [SerializeField] private GameObject _stageSelectMulti;
    private void Start()
    {
        ChangeScreen(0);
    }
    /// <summary>
    /// 画面の切り替え
    /// </summary>
    /// <param name="number">切り替える画面</param>
    public void ChangeScreen(int number)
    {
        _title.SetActive(number == 0);
        _roomJoin.SetActive(number == 1);
        _stageSelectSingle.SetActive(number == 2);
        _stageSelectMulti.SetActive(number == 3);
    }

}
