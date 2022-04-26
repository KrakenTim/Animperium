using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;

    public GameObject LoadingPanel;
    public float MinLoadTime;
    public float fadeOutTime = 0.25f;
    public float fadeInTime = 0.25f;

    public GameObject LoadingWheel;
    public float WheelSpeed;

    private string targetScene;
    private bool IsLoading;

    //public Slider slider;
    //public Text progressText;

    private Image LoadingPanelBackground;
    private Color originalColor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadingPanelBackground = LoadingPanel.GetComponent<Image>();
        originalColor = LoadingPanelBackground.color;

        LoadingPanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        IsLoading = true;

        LoadingPanel.SetActive(true);
        StartCoroutine(SpinWheelRoutine());

        Color changingColor = originalColor;
        float fadeInElapsed = 0;
        while (fadeInElapsed < fadeOutTime)
        {
            // slowly shift from transparent (0) to original alpha
            changingColor.a = originalColor.a * (fadeInElapsed / fadeOutTime);
            LoadingPanelBackground.color = changingColor;

            fadeInElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        LoadingPanelBackground.color = originalColor;

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        float elapsedLoadTime = 0f;

        while (!op.isDone)
        {
            //float progress = Mathf.Clamp01(op.progress / .9f);

            //slider.value = progress;
            //progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            Debug.Log(Mathf.Round(op.progress * 100));

            elapsedLoadTime += Time.unscaledDeltaTime;
            yield return null;
        }

        while (elapsedLoadTime < MinLoadTime)
        {
            elapsedLoadTime += Time.unscaledDeltaTime;
            yield return null;
        }

        float fadeOutElapsed = 0f;
        changingColor = originalColor;

        while (fadeOutElapsed < fadeOutTime)
        {
            // slowly shift from origional alpha to transparent (0)
            changingColor.a = originalColor.a * (1f - fadeOutElapsed / fadeOutTime) ;
            LoadingPanelBackground.color = changingColor;

            fadeOutElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (LoadingPanel)
            LoadingPanel.SetActive(false);

        LoadingPanelBackground.color = originalColor;

        IsLoading = false;
    }

    private IEnumerator SpinWheelRoutine()
    {
        while (IsLoading)
        {
            if (LoadingWheel == null)
                yield break;

            LoadingWheel.transform.Rotate(0, 0, -WheelSpeed);
            yield return null;
        }
    }
}