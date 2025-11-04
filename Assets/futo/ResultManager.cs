using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;
using DG.Tweening;
public class ResultManager : MonoBehaviour
{
    [Header("タイム設定")]
    [SerializeField] private float _threeStarTime;
    [SerializeField] private float _twoStarTime;
    [SerializeField] private float _oneStarTime;

    [Header("コンポーネント設定")]
    [SerializeField] private GameObject _resultPnanel;
    [SerializeField] private Image[] _starImage;
    [SerializeField] private Button _nextButton;
    [SerializeField] private GameManager _gameManager;
    private List<Animator> _animators;
    private int _starCount;
    [Header("アニメーション")]
    [SerializeField] private float _moveduraion;
    [SerializeField] private Ease _moveEase;
    private void Start()
    {
        _animators = new List<Animator>();
        for (int i = 0; i < _starImage.Length; i++)
        {
            _animators.Add(_starImage[i].GetComponent<Animator>());
        }
        if(_gameManager == null)
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }

        _nextButton.onClick.AddListener(_gameManager.GameClear);
    }
    [PunRPC]
    public void ShowResult(float clearTime)
    {
        _resultPnanel.SetActive(true);
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
    }
    private IEnumerator ShowStar(int starCount)
    {
        for (int i = 0;i < starCount;i++)
        {
            yield return new WaitForSeconds(1);
            _animators[i].SetBool("ShowStar", true);
        }
    }
}
