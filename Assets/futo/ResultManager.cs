using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [Header("タイム設定")]
    [SerializeField] private float _threeStarTime;
    [SerializeField] private float _twoStarTime;
    [SerializeField] private float _oneStarTime;

    [Header("コンポーネント設定")]
    [SerializeField] private GameObject _resultPnanel;
    [SerializeField] private Image[] _starImage;
    private List<Animator> _animators;

    private int _starCount;
    [SerializeField] float Time;

    private void Start()
    {
        _animators = new List<Animator>();
        for (int i = 0; i < _starImage.Length; i++)
        {
            _animators.Add(_starImage[i].GetComponent<Animator>());
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShowResult(Time);
        }
    }
    public void ShowResult(float clearTime)
    {
        foreach (Image star in _starImage)
        {
            star.enabled = false;
        }

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

        _resultPnanel.SetActive(true);

        StartCoroutine(ShowStar(_starCount));
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
