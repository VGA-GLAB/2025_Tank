using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneMove : MonoBehaviour
{
    [Header("UI")]
    public Image fadeImage;          // 全画面に配置した黒いImage
    public float fadeDuration = 1f;  // フェード時間

    private void Awake()
    {
        // fadeImageが未設定なら自動生成
        if (fadeImage == null)
        {
            GameObject canvasGO = new GameObject("FadeCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // 他UIより前に表示

            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject imageGO = new GameObject("FadeImage");
            imageGO.transform.SetParent(canvasGO.transform, false);
            fadeImage = imageGO.AddComponent<Image>();
            fadeImage.color = Color.black;

            // 全画面に伸ばす
            RectTransform rt = fadeImage.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }

    private void Start()
    {
        //// 起動時はフェードイン
        //if (fadeImage != null)
        //{
        //    fadeImage.color = new Color(0, 0, 0, 1f);
        //    StartCoroutine(FadeIn());
        //}
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null) yield break;

        float startAlpha = fadeImage.color.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }
}
