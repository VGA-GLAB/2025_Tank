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
    private void OnEnable()
    {
        if(_bgmVolSlider!= null)
        {
            _bgmVolSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            _bgmVolSlider.onValueChanged.Invoke(_bgmVolSlider.value);
        }
        if(_seVolSlider != null)
        {
            _seVolSlider.onValueChanged.AddListener(OnSEVolumeChanged);
            _seVolSlider.onValueChanged.Invoke(_seVolSlider.value);
        }
    }

    private void Start()
    {
        //_bgmVolSlider.value = 1f;
        //_seVolSlider.value = 0.3f;
    }

    private void OnBGMVolumeChanged(float value)
    {
        CRIAudioManager.BGM.SetVolume(value);
        // BGMが再生中の場合、音量を即座に反映させる
        if (CRIAudioManager.BGM.IsPlaying)
        {
            CRIAudioManager.BGM.UpdateAll();
        }
    }

    private void OnSEVolumeChanged(float value)
    {
        Debug.Log("se");
        CRIAudioManager.SE.SetVolume(value);
        // SEが再生中の場合、音量を即座に反映させる
        if (CRIAudioManager.SE.IsPlaying)
        {
            CRIAudioManager.SE.UpdateAll();
        }
    }
}
