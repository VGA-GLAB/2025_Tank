using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class StageListView : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private StageData[] _stageList;
    [SerializeField] private Button[] _stageSelectButtons;
    [SerializeField] private string[] _stageSceneName;
    [SerializeField] private TitleUIManager _titleUIManager;
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI _stageNumber;
    [SerializeField] private LocalizeStringEvent _stageName;
    [SerializeField] private Image _stageImage;
    [SerializeField] private TextMeshProUGUI _normalEnemy;
    [SerializeField] private TextMeshProUGUI _laserEnemy;
    [SerializeField] private TextMeshProUGUI _buckshotEnemy;
    [SerializeField] private TextMeshProUGUI _flankingEnemy;
    [SerializeField] private TextMeshProUGUI _fixedEnemy;
    [SerializeField] private TextMeshProUGUI _bossEnemy;
    [SerializeField] private SelectAnimation _defaultSelectAnimation;
    private SelectAnimation _selectAnimation;
    public string SelectStage { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_titleUIManager == null)
        {
            _titleUIManager = FindAnyObjectByType<TitleUIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShowInfo(int index)
    {
        Debug.Log(index);
        if (index >= _stageList.Length) return;

        SelectStage = _stageSceneName[index];

        _stageNumber.text = _stageList[index].Number;
        _stageName.StringReference = _stageList[index].Name;
        _stageImage.sprite = _stageList[index].Image;

        _normalEnemy.text = "×" + _stageList[index].NormalEnemy;
        _laserEnemy.text = "×" + _stageList[index].LaserEnemy;
        _buckshotEnemy.text = "×" + _stageList[index].BuckshotEnemy;
        _flankingEnemy.text = "×" + _stageList[index].FlankingEnemy;
        _fixedEnemy.text = "×" + _stageList[index].FixedEnemy;
        _bossEnemy.text = "×" + _stageList[index].BossEnemy;
    }
    public async void ShowList()
    {
        await UniTask.Yield();
        ShowInfo(0);
        _selectAnimation?.SetLoopAnimation(false);
        _defaultSelectAnimation.SetLoopAnimation(true);
        _selectAnimation = _defaultSelectAnimation;
    }
    public void ChangeSelect(GameObject obj)
    {
        Debug.Log(obj);
        if (!obj.TryGetComponent(out SelectAnimation selectAnimation))
        {
            return;
        }
        if (_selectAnimation != null)
        {
            _selectAnimation.SetLoopAnimation(false);
        }
        _selectAnimation = selectAnimation;
        selectAnimation.SetLoopAnimation(true);

    }
    public void SetStageClearData()
    {
        int i = 0;
        foreach (var button in _stageSelectButtons)
        {
            int index = i;
            GameObject buttonObj = button.gameObject;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                ShowInfo(index);
                ChangeSelect(buttonObj);
                _titleUIManager.OnButtonClick();
            });
            i++;
        }
        ShowInfo(0);
        ChangeSelect(_stageSelectButtons[0].gameObject);
        SelectStage = _stageSceneName[0];

        if (_stageSelectButtons.Length != _stageSceneName.Length) return;

        for (i = 0; i < _stageSelectButtons.Length; i++)
        {
            if (i == 0)
            {
                _stageSelectButtons[i].interactable = true;
                continue;
            }
            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                int stageClear = PlayerPrefs.GetInt(_stageSceneName[i - 1]);
                _stageSelectButtons[i].interactable = stageClear == 1;

                if (PhotonNetwork.IsConnected)
                    NetworkCore.SetNetValue(_stageSceneName[i - 1], stageClear);
            }
            else
            {
                _stageSelectButtons[i].interactable = NetworkCore.GetNetValue(_stageSceneName[i - 1], out _) == 1;
            }
        }
    }
    public void ResetData()
    {
        foreach (var name in _stageSceneName)
        {
            PlayerPrefs.SetInt(name, 0);
        }
        SetStageClearData();
    }
}
