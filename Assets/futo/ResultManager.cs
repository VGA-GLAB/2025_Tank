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

    private float _timer;
    private int starCount;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            ShowResult();
        }
    }

    public void ShowResult()
    {
        foreach (Image star in _starImage)
        {
            star.enabled = false;
        }

        switch (true)
        {
            case bool _ when _timer <= _threeStarTime:
                starCount = 3;
                break;
            case bool _ when _timer <= _twoStarTime:
                starCount = 2;
                break;
            case bool _ when _timer <= _oneStarTime:
                starCount = 1;
                break;
            default:
                starCount = 0;
                break;
        }

        for (int i = 0; i < starCount; i++)
        {
            _starImage[i].enabled = true;
        }

        _resultPnanel.SetActive(true);
    }
}
