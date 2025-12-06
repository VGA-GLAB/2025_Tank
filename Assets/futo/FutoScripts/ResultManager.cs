using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using DG.Tweening;
using TMPro;
using Photon.Realtime;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using UnityEditor.Localization.Plugins.XLIFF.Common;
public class ResultManager : MonoBehaviourPunCallbacks
{
    [Header("タイム設定")]
    [SerializeField] private float _threeStarTime;
    [SerializeField] private float _twoStarTime;
    [SerializeField] private float _oneStarTime;

    [Header("コンポーネント設定")]
    [SerializeField] private GameObject _resultPnanel;
    [SerializeField] private GameObject _detailPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI[] _detailText;
    [SerializeField] private Image[] _starImage;
    [SerializeField] private Button _titleButton;
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _titleGameOverButton;
    [SerializeField] private Button _reStart;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private GameManager _gameManager;
    private int _starCount;
    [Header("アニメーション")]
    [SerializeField] private float _moveduraion;
    [SerializeField] private Ease _moveEase;
    private void Start()
    {
        if(_gameManager == null)
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }

        _titleButton.onClick.AddListener(_gameManager.GameOver);
        _replayButton.onClick.AddListener(() => _gameManager.GetComponent<PhotonView>().RPC("Retry", RpcTarget.All));
        _nextButton.onClick.AddListener(_gameManager.GameClear);

        _titleGameOverButton.onClick.AddListener(_gameManager.GameOver);
        _reStart.onClick.AddListener(_gameManager.ReStart);

        _detailText[0].text = $"{GetTimeString((int)_oneStarTime)}以下";
        _detailText[1].text = $"{GetTimeString((int)_twoStarTime)}以下";
        _detailText[2].text = $"{GetTimeString((int)_threeStarTime)}以下";

        _gameOverPanel.SetActive(false);
        _detailPanel.SetActive(false);
    }
   
    [PunRPC]
    public void ShowResult(int clearTime)
    {
        OnMasterClientSwitched(null);
        _resultPnanel.SetActive(true);

        _timeText.text = GetTimeString(clearTime);

        if(!_resultPnanel.TryGetComponent(out RectTransform pnanelRect))
        {
            StartCoroutine(ShowStar(_starCount));
        }
        pnanelRect.anchoredPosition = Vector2.up * 1030;

        pnanelRect.DOAnchorPosY(0, 1f).SetEase(_moveEase)
            .OnComplete(() =>
        {
            switch (true)
            {
                case bool _ when clearTime <= _threeStarTime:
                    _starCount = 3;
                    break;
                case bool _ when clearTime <= _twoStarTime:
                    _starCount = 2;
                    break;
                case bool _ when clearTime <= _oneStarTime:
                    _starCount = 1;
                    break;
                default:
                    _starCount = 0;
                    break;
            }


            StartCoroutine(ShowStar(_starCount));
        });
        CRIAudioManager.BGM.Stop();
    }

    [PunRPC]
    public void ShowGameOverResult()
    {
        _titleGameOverButton.interactable = PhotonNetwork.IsMasterClient;
        _reStart.interactable= PhotonNetwork.IsMasterClient;
        _gameOverPanel.gameObject.SetActive(true);

        _gameOverPanel.TryGetComponent(out RectTransform pnanelRect);

        pnanelRect.anchoredPosition = Vector2.zero;

        pnanelRect.DOAnchorPosY(-1030f, 1f).SetEase(_moveEase);
    }

    private IEnumerator ShowStar(int starCount)
    {
        for (int i = 0;i < starCount;i++)
        {
            yield return new WaitForSeconds(1);
            //Sound: 星
            _starImage[i].transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            _starImage[i].transform.DOScale(new Vector3(1.1f, 1.1f, 1f), 1f).SetEase(Ease.OutBack).SetUpdate(true);
            CRIAudioManager.SE.Play("SE", "Star_get");

        }
    }
    private string GetTimeString(int secondTime)
    {
        int minute = Mathf.FloorToInt(secondTime / 60);
        int second = Mathf.FloorToInt(secondTime % 60);
        return $"{minute:00}:{second:00}";
    }
    public void ShowDetail()
    {
        _detailPanel.SetActive(!_detailPanel.activeSelf);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _titleButton.interactable = PhotonNetwork.IsMasterClient;
        _replayButton.interactable = PhotonNetwork.IsMasterClient;
        _nextButton.interactable = PhotonNetwork.IsMasterClient;

        _titleGameOverButton.interactable = PhotonNetwork.IsMasterClient;
        _reStart.interactable = PhotonNetwork.IsMasterClient;
    }
}
