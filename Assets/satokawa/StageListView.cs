using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageListView : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _buttonPrefab;
    [SerializeField] private StageData[] _stageList;
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI _stageNumber;
    [SerializeField] private TextMeshProUGUI _stageName;
    [SerializeField] private Image _stageImage;
    [SerializeField] private TextMeshProUGUI _normalEnemy;
    [SerializeField] private TextMeshProUGUI _laserEnemy;
    [SerializeField] private TextMeshProUGUI _buckshotEnemy;
    [SerializeField] private TextMeshProUGUI _flankingEnemy;
    [SerializeField] private TextMeshProUGUI _bossEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowInfo(int index)
    {
        if (index >= _stageList.Length) return;

        _stageNumber.text = _stageList[index]._number;
        _stageName.text = _stageList[index]._name;
        _stageImage.sprite = _stageList[index]._image;
        
        _normalEnemy.text = "×" + _stageList[index]._normalEnemy;
        _laserEnemy.text = "×" + _stageList[index]._laserEnemy;
        _buckshotEnemy.text = "×" + _stageList[index]._buckshotEnemy;
        _flankingEnemy.text = "×" + _stageList[index]._flankingEnemy;
        _bossEnemy.text = "×" + _stageList[index]._bossEnemy;
    }
    public void ShowList()
    {
        DeleteChild();
        int index = 0;
        foreach(var stage in _stageList)
        {
            GameObject newObject = Instantiate(_buttonPrefab.gameObject, _content);
            int i = index;
            newObject.GetComponent<Button>().onClick.AddListener(() => ShowInfo(i)); 
            if(newObject.TryGetComponent(out Image image))
            {
                image.sprite = stage._image;
            }
            index++;
        }
        ShowInfo(0);
    }
    public void DeleteChild()
    {
        for(int i = _content.childCount - 1; i >= 0; i--)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
    }
}
