using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PauseControl : MonoBehaviourPunCallbacks
{
    [SerializeField] private RectTransform _panel;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private InGameNetworkManager _inGameNetworkManager;
    [SerializeField] private float _clausePositionY;
    [SerializeField] private float _moveDuration;
    [SerializeField] private TutorialSlideShow _tutorial;
    [Header("ボタン")]
    [SerializeField] private Button _buttonResume;
    [SerializeField] private Button _buttonRestart;
    [SerializeField] private Button _buttonTitle;
    [SerializeField] private Button _buttonDisconnect;

    private Tween _panelTween;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_gameManager == null)
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }
        if(_inGameNetworkManager == null)
        {
            _inGameNetworkManager = FindAnyObjectByType<InGameNetworkManager>();
        }
        if(_tutorial == null)
        {
            _tutorial = FindAnyObjectByType<TutorialSlideShow>();
        }
       _panel.gameObject.SetActive(false);
        _buttonResume.onClick.RemoveAllListeners();
        _buttonRestart.onClick.RemoveAllListeners();
        _buttonTitle.onClick.RemoveAllListeners();
        _buttonDisconnect.onClick.RemoveAllListeners();

        _buttonResume.onClick.AddListener(() => _gameManager.ClausePause());
        _buttonRestart.onClick.AddListener(() => _gameManager.KillAllPlayer());
        _buttonTitle.onClick.AddListener(() => _gameManager.GameOver()); 
        _buttonDisconnect.onClick.AddListener(() => _inGameNetworkManager.ReturnToTitle()); 
    }
    public bool ShowPanel(bool b)
    {
        if (_tutorial.gameObject.activeSelf) return false;

        _panel.gameObject.SetActive(b);

        if(_panelTween != null)
        {
            _panelTween.Complete();
            _panelTween.Kill();
        }

       _panelTween = _panel.DOAnchorPos(Vector2.up * (b ? 0 : _clausePositionY), _moveDuration).SetUpdate(true);

        if (b)
        {
            SetButtonData();
        }

        return true;
    }
    public bool IsShow()
    {
        return _panel.gameObject.activeSelf;
    }
    private void SetButtonData()
    {
        _buttonDisconnect.gameObject.SetActive(!PhotonNetwork.OfflineMode);
        _buttonTitle.gameObject.SetActive(PhotonNetwork.OfflineMode);

        OnMasterClientSwitched(null);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _buttonRestart.interactable = PhotonNetwork.IsMasterClient;
    }
}
