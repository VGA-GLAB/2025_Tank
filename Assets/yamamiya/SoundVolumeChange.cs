using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeChanger : MonoBehaviour
{
    [SerializeField] private Slider _bgmVolSlider;
    [SerializeField] private Slider _seVolSlider;

    private void Awake()
    {
        if(_bgmVolSlider == null)
        {
            Debug.LogError("BGM用のスライダーがアタッチされていません。");
        }

        if (_seVolSlider == null)
        {
            Debug.LogError("SE用のスライダーがアタッチされていません。");
        }
    }

    private void Start()
    {
        _bgmVolSlider.value = 1f;
        _seVolSlider.value = 1f;

        _bgmVolSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        _seVolSlider.onValueChanged.AddListener(OnSEVolumeChanged);
    }

    private void OnBGMVolumeChanged(float value)
    {
        CRIAudioManager.BGM.SetVolume(value);
    }

    private void OnSEVolumeChanged(float value)
    {
        CRIAudioManager.SE.SetVolume(value);
    }
}
