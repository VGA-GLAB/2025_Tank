using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeChanger : MonoBehaviour
{
    [SerializeField] private Slider _bgmVolSlider;
    [SerializeField] private Slider _seVolSlider;

    // PlayerPrefsのキー
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";

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

        // 保存されている音量を読み込み、スライダーに反映
        if (_bgmVolSlider != null)
        {
            float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
            _bgmVolSlider.value = savedBGMVolume;
        }
        if (_seVolSlider != null)
        {
            float savedSEVolume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.3f);
            _seVolSlider.value = savedSEVolume;
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
        // 音量設定を保存
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
        
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
        // 音量設定を保存
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, value);
        PlayerPrefs.Save();
        
        // SEが再生中の場合、音量を即座に反映させる
        if (CRIAudioManager.SE.IsPlaying)
        {
            CRIAudioManager.SE.UpdateAll();
        }
    }
}
