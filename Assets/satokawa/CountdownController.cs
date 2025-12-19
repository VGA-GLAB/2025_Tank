using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

public class CountdownController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private LocalizeStringEvent _countdownTextLocalize;
    [SerializeField] private float _upPositionY = 25f;
    [SerializeField] private float _maxScale = 25f;
    [SerializeField] private float _minScale = 7f;
    [Header("数字アニメーション")]
    [SerializeField] private float _fadeInTime = 0.5f; 
    [SerializeField] private float _fadeOutTime = 0.3f; 
    [SerializeField] private float _waitTime = 0.2f;
    [SerializeField] private float _countScale = 5f;
    [Space]
    [SerializeField] private Color _normalColor = new Color(1f, 0.95f, 0.8f); // 柔らかベージュ
    [SerializeField] private Color _lastColor = new Color(1f, 1f, 0.9f);      // やや明るめ

    public void RequestStartCountdown(string callback,PhotonView view)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(StartCountdownRPC), RpcTarget.All, callback,view.ViewID);
        }
    }

    [PunRPC]
    public void StartCountdownRPC(string callback,int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if(view == null)
        {
            Debug.LogError("don't find viewID");
        }
        if (!_countdownText.TryGetComponent(out RectTransform rect))
        {
            if (PhotonNetwork.IsMasterClient && callback != "")
            {
                view.RPC(callback, RpcTarget.All);
            }
            return;
        }

        Sequence sequence = DOTween.Sequence();
        _countdownText.text = "";
        _countdownText.gameObject.SetActive(true);
        _countdownTextLocalize.enabled = false;
        

        //===============================
        // 数字
        //===============================
        void AddStep(string text, bool isLast = false)
        {
            sequence.AppendCallback(() =>
            {
                _countdownText.color = isLast ? _lastColor : _normalColor;
                rect.localScale = Vector3.one * _countScale; 
                // 位置と透明化
                rect.anchoredPosition = new Vector2(0f, _upPositionY);
                var c = _countdownText.color;
                c.a = 0f;
                _countdownText.color = c;
                _countdownText.text = text;

            });

            //sequence.AppendCallback(() =>
            //{
            //    _aud.PlayOneShot(_countDownSe);
            //});

            // フェードイン
            sequence.Append(
                _countdownText.DOFade(1f, _fadeInTime)
                    .SetEase(Ease.OutQuart)
            );

            // 同時に中央へ移動
            sequence.Join(
                rect.DOAnchorPos(Vector2.zero, _fadeInTime)
                    .SetEase(Ease.OutQuart)
            );
                
            // 軽いフェードアウト
            sequence.Append(
                _countdownText.DOFade(0f, _fadeOutTime)
                    .SetEase(Ease.InQuart)
            );
            sequence.AppendCallback(() =>
            {
                _countdownText.text = "";
            });
            // 余韻
            sequence.AppendInterval(_waitTime);
        }

        // 3,2,1
        AddStep("3");
        AddStep("2");
        AddStep("1");
        
        //===============================
        // スタート
        //===============================
        sequence.AppendCallback(() =>
        {
            CRIAudioManager.SE.Play("SE", "start");
            _countdownText.color = _lastColor;
            var c = _countdownText.color;
            c.a = 0.5f;
            _countdownText.color = c;

            rect.localScale = Vector3.one * _maxScale;
            _countdownTextLocalize.enabled = true;
            //_countdownText.text = "スタート";

            //_aud.PlayOneShot(_startSe);
        });

        //sequence.AppendCallback(() =>
        //{
            
        //});
        //完了コールバック
        sequence.AppendCallback(() =>
        {
            if (PhotonNetwork.IsMasterClient && callback != "")
            {
                view.RPC(callback, RpcTarget.All);
            }
        });
        //スタートの続き
        float downDuration = 0.6f;
        sequence.Append(
            rect.DOScale(Vector3.one * _minScale, downDuration)
                .SetEase(Ease.OutQuart)
        );
        sequence.Join(
            _countdownText.DOFade(1f, downDuration)
                    .SetEase(Ease.OutQuart)
        );

        sequence.AppendInterval(0.1f);
        sequence.Append(
            _countdownText.DOFade(0f, 1- downDuration)
                .SetEase(Ease.InQuart)
        );
        sequence.AppendCallback(() =>
        {
            _countdownText.color = _normalColor;
            _countdownText.alpha = 1f;
            _countdownText.gameObject.SetActive(false);
        });

        CRIAudioManager.SE.Play("SE", "count");
        sequence.Play();

    }

}