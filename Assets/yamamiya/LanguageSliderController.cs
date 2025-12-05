using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageSliderController : MonoBehaviour
{
    [SerializeField] private LocaleChanger _localeChanger;
    [SerializeField] private Slider _slider;
    private Dictionary<int, string> _languageMap = new Dictionary<int, string>
    {
        {0, "en"},
        {1, "ja"},
    };

    private void Awake()
    {
        if(_slider == null)
        {
            Debug.LogError("Slider is not assigned in LanguageSliderController.");
            return;
        }

        if(_localeChanger == null)
        {
            Debug.LogError("LocaleChanger is not assigned in LanguageSliderController.");
            return;
        }
    }

    private void Start()
    {
        var currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;
        foreach (var pair in _languageMap)
        {
            if (pair.Value == currentLocale)
            {
                _slider.value = pair.Key;
                break;
            }
        }
    }

    public void OnSliderValueChanged(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        if (_languageMap.TryGetValue(intValue, out string locale))
        {
            _localeChanger.Chanage(locale);
        }
    }
}
