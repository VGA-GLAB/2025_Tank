using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderColorDefaultGradient : MonoBehaviour
{
    private Slider slider;

    // �����o�[�ϐ��Ńf�t�H���g�̐ԁ������΃O���f�[�V������p��
    [SerializeField]
    private Gradient gradient = new Gradient
    {
        colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.red, 0f),
            new GradientColorKey(Color.yellow, 0.5f),
            new GradientColorKey(Color.green, 1f)
        },
        alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        }
    };

    private Image fillImage;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
        fillImage = slider.fillRect.GetComponent<Image>();
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(UpdateSliderColor);
        UpdateSliderColor(slider.value); // �����l���f
    }

    private void UpdateSliderColor(float value)
    {
        value = Mathf.Abs(value);
        if (slider.maxValue <= 0)
        {
            fillImage.color = Color.red;
            return;
        }

        float t = Mathf.Clamp01(value / slider.maxValue);
        fillImage.color = gradient.Evaluate(t);
    }
}
