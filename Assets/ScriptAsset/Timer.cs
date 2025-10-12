using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public enum TimerMode { Stopwatch, Countdown }

    [Header("Timer Settings")]
    public TimerMode mode = TimerMode.Stopwatch; // Stopwatch: 0����J�E���g�A�b�v, Countdown: duration����0��
    public float duration = 5f;                  // Countdown���̃X�^�[�g����
    public bool isLoop = false;                  // ���[�v���邩
    [Range(0, 3)] public int decimalPlaces = 1; // �����_�\������

    private float timer = 0f;
    private bool isRunning = false;

    [Header("UI")]
    public TMP_Text uiText;                       // �\���pTextMeshPro

    [Header("Events")]
    public UnityEvent onComplete;                 // �^�C�}�[�I�����ɌĂ΂��
    public void Start()
    {
        StartTimer();
    }

    // �^�C�}�[�J�n
    public void StartTimer()
    {
        timer = (mode == TimerMode.Countdown) ? duration : 0f;
        isRunning = true;
        UpdateText();
    }

    // �^�C�}�[��~
    public void StopTimer()
    {
        isRunning = false;
    }

    void Update()
    {
        if (!isRunning) return;

        if (mode == TimerMode.Stopwatch)
        {
            timer += Time.deltaTime;
            if (timer >= duration && duration > 0f)
            {
                TimerComplete();
            }
        }
        else // Countdown
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = 0f;
                TimerComplete();
            }
        }

        UpdateText();
    }

    private void TimerComplete()
    {
        onComplete?.Invoke();

        if (isLoop)
        {
            timer = (mode == TimerMode.Countdown) ? duration : 0f;
        }
        else
        {
            isRunning = false;
        }
    }

    private void UpdateText()
    {
        if (uiText != null)
        {
            string format = "F" + decimalPlaces; // �����_�����w��
            uiText.text = timer.ToString(format);
        }
    }
}
