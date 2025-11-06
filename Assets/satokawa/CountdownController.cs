using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _minScale;
    [SerializeField, Range(0, 1)] private float _moveSpeed;
    [SerializeField] private Ease _easing;
    [SerializeField] private Color _normalColor = new Color(1f, 0.95f, 0.8f); // 柔らかベージュ
    [SerializeField] private Color _lastColor = new Color(1f, 1f, 0.9f);      // やや明るめ

    public void RequestStartCountdown(UnityAction callback)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(StartCountdownRPC), RpcTarget.All);
            _callback = callback;
        }
    }

    private static UnityAction _callback;
    [PunRPC]
    public void StartCountdownRPC()
    {
        if (!_countdownText.TryGetComponent(out RectTransform rect))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _callback?.Invoke();
            }
            return;
        }

        Sequence sequence = DOTween.Sequence();
        _countdownText.gameObject.SetActive(true);

        void AddStep(string text, bool isLast = false)
        {
            sequence.AppendCallback(() =>
            {
                _countdownText.text = text;
                _countdownText.color = isLast ? _lastColor : _normalColor;
                rect.localScale = Vector3.one * _maxScale;
            });
            sequence.Append(rect.DOScale(Vector3.one * _minScale, _moveSpeed)
                .SetEase(_easing));
            sequence.AppendInterval(1f - _moveSpeed);
        }

        AddStep("3");
        AddStep("2");
        AddStep("1");
        AddStep("スタート！", true);

        sequence.AppendCallback(() =>
        {
            _countdownText.DOFade(0f, 0.4f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _countdownText.color = _normalColor;
                    _countdownText.alpha = 1f;
                    _countdownText.gameObject.SetActive(false);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        _callback?.Invoke();
                    }
                });
        });

        sequence.Play();
    }
}