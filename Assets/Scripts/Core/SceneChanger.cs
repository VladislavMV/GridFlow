using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    [SerializeField] private CanvasGroup faderCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(Fade(0f));
    }

    public void ChangeScene(int sceneIndex)
    {
        StartCoroutine(FadeAndLoad(sceneIndex));
    }

    private IEnumerator FadeAndLoad(int sceneIndex)
    {
        yield return StartCoroutine(Fade(1f));
        yield return SceneManager.LoadSceneAsync(sceneIndex);
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = faderCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            faderCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        faderCanvasGroup.alpha = targetAlpha;
        faderCanvasGroup.blocksRaycasts = (targetAlpha == 1f);
    }
}