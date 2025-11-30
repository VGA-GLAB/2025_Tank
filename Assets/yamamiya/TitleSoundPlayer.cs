using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TitleSoundPlayer : MonoBehaviour
{
    [SerializeField] private Slider _bgmVolSlider;
    [SerializeField] private Slider _seVolSlider;

    private void Awake()
    {
        if(_bgmVolSlider == null)
        {
            Debug.LogWarning("BGMボリュームスライダーが設定されていません。");
        }
        if(_seVolSlider == null)
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
        CRIAudioManager.BGM.SetVolume(_bgmVolSlider.value);
        CRIAudioManager.SE.SetVolume(_seVolSlider.value);

        // Sound:タイトル画面
    }
}
