using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;

    public GameObject LoadingPanel;
    public float MinLoadTime;

    public GameObject LoadingWheel;
    public float WheelSpeed;

    private string targetScene;
    private bool IsLoading;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

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

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        float elapsedLoadTime = 0f;

        while (!op.isDone)
        {
            elapsedLoadTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedLoadTime < MinLoadTime)
        {
            elapsedLoadTime += Time.deltaTime;
            yield return null;
        }

        if (LoadingPanel)
        LoadingPanel.SetActive(false);

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


    /*
        public GameObject loadingScreen;
        public Slider slider;
        public Text progressText;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        // use int instead of string to use the Unity Scene Index
        public void LoadLevel(string NormanMapGeneration)
        {
            StartCoroutine(LoadAsynchronously(NormanMapGeneration));
        }

        IEnumerator LoadAsynchronously (string NormanMapGeneration)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(NormanMapGeneration);

            loadingScreen.SetActive(true);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);

                slider.value = progress;
                progressText.text = progress * 100f + "%";

                yield return null;
            }
        }*/
}
