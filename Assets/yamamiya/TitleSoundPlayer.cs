using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSoundPlayer : MonoBehaviour
{
    [SerializeField] private Slider _bgmVolSlider;
    [SerializeField] private Slider _seVolSlider;

    /// <summary>
    /// BGMを流す対象のシーン名
    /// NOTE Steamに出す用のビルドをする際は消す
    /// </summary>
    [SerializeField] private string _targetSceneName = "Title";

    private void Awake()
    {
        if (_bgmVolSlider == null)
        {
            Debug.LogWarning("BGMボリュームスライダーが設定されていません。");
        }
        if (_seVolSlider == null)
        {
            Debug.LogWarning("SEボリュームスライダーが設定されていません。");
        }

        CRIAudioManager.Initialize();
    }

    private async void Start()
    {
        // CRIAudioManagerの初期化完了を待機
        await UniTask.WaitUntil(() => CRIAudioManager.IsReady);
        // スライダーの初期値に基づいて音量を設定
        if (_bgmVolSlider != null)
        {
            CRIAudioManager.BGM.SetVolume(_bgmVolSlider.value);
        }
        if (_seVolSlider != null)
        {
            CRIAudioManager.SE.SetVolume(_seVolSlider.value);
        }

        // シーン名が一致しない場合はBGMを再生しない
        if (_targetSceneName != SceneManager.GetActiveScene().name) return;

        CRIAudioManager.BGM.Play("BGM", "bgm_title");
    }
}
