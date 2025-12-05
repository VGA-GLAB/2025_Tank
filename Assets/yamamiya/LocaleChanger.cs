using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocaleChanger : MonoBehaviour
{
    /// <summary>
    /// 指定し言語コードに切り替える
    /// </summary>
    /// <param name="locale"></param>
    public void Chanage(string locale)
    {
        var _ = ChangeSelectedLocale(locale);
    }

    /// <summary>
    /// 指定したロケール（言語コード）に切り替える
    /// </summary>
    /// <param name="locale"></param>
    /// <returns></returns>
    private async UniTask ChangeSelectedLocale(string locale)
    {
        // 指定されたコードからLocaleを生成し、選択中のロケールとして設定する
        LocalizationSettings.SelectedLocale = Locale.CreateLocale(locale);
        await UniTask.WaitUntil(() => LocalizationSettings.SelectedLocale.Identifier.Code == locale);
    }
}
