using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public enum TimerMode { Stopwatch, Countdown }

    [Header("Timer Settings")]
    public TimerMode mode = TimerMode.Stopwatch; // Stopwatch: 0からカウントアップ, Countdown: durationから0へ
    public float duration = 5f;                  // Countdown時のスタート時間
    public bool isLoop = false;                  // ループするか
    [Range(0, 3)] public int decimalPlaces = 1; // 小数点表示桁数

    private float timer = 0f;
    private bool isRunning = false;

    [Header("UI")]
    public TMP_Text uiText;                       // 表示用TextMeshPro

    [Header("Events")]
    public UnityEvent onComplete;                 // タイマー終了時に呼ばれる
    public void Start()
    {
        StartTimer();
    }

    // タイマー開始
    public void StartTimer()
    {
        timer = (mode == TimerMode.Countdown) ? duration : 0f;
        isRunning = true;
        UpdateText();
    }

    // タイマー停止
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
            string format = "F" + decimalPlaces; // 小数点桁数指定
            uiText.text = timer.ToString(format);
        }
    }
}
