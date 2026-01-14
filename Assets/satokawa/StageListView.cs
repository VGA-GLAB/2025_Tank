using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class StageListView : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _buttonPrefab;
    [SerializeField] private StageData[] _stageList;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_titleUIManager == null)
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
        if (index >= _stageList.Length) return;

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
        if(!obj.TryGetComponent(out SelectAnimation selectAnimation ))
        {
            return;
        }
        if(_selectAnimation != null)
        {
            _selectAnimation.SetLoopAnimation(false);
        }
        _selectAnimation = selectAnimation;
        selectAnimation.SetLoopAnimation(true);
        
    }
    public void DeleteChild()
    {
        for(int i = _content.childCount - 1; i >= 0; i--)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
    }
}
