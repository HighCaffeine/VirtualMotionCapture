using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Menu,
    Loading,
    Game,


    Count
}

public class SceneController : GenericSingleton<SceneController>
{
    private new void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }

    public static bool IsLoadGameScene => isLoadGameScene;

    private static bool isLoadGameScene = false;

    public void GoToScene(string sceneName)
    {
        StartCoroutine(StartLoad(sceneName));
    }

    public delegate void LoadingBarProgress(float progress);
    public LoadingBarProgress loadingBarProgress;

    IEnumerator StartLoad(string sceneName)
    {
        SoundManager.Instance.PauseBGM();

        SceneManager.LoadSceneAsync("Loading");

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            loadingBarProgress?.Invoke(async.progress);

            if (async.progress >= 0.9f)
            {
                loadingBarProgress?.Invoke(1f);
                
                yield return new WaitForSeconds(1f);
                
                async.allowSceneActivation = true;

                isLoadGameScene = true;

                //해당 부분 추후 맵 관련하여 결정될 경우
                //현재 계절 / 맵 단계에 따른 BGM 변경
                SoundManager.Instance.PlaySound(SoundManager.BGM.BGM_WakEnt_1.ToString(), true);

                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
}
