using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadingImageController : MonoBehaviour
{
    [SerializeField] private Image loadImage;
    [SerializeField] private Image loadBar;
    [SerializeField] private TextMeshProUGUI tmiTMP; 


[Header("TMI")]
    [SerializeField] private string[] tmi;
    [SerializeField] private string[] loadText;
    [SerializeField] private float tmiChangeDelay;
    [SerializeField] private float tmiWordingDelay;

    [Header("LoadingImage")]
    [SerializeField] private Sprite[] loadImages;
    [SerializeField] private float imageChangeDelay;
    [SerializeField] private float fadeTimeDelay;

    private Color fadeColor;

    private void OnEnable()
    {
        fadeColor = new Color(1f, 1f, 1f, 1f);

        SoundManager.Instance.PlaySound(SoundManager.BGM.BGM_MaSaeDol.ToString(), false);
        SceneController.Instance.loadingBarProgress = LoadBarProgress;
    }

    private void Start()
    {
        //로딩 이미지 및 ,tmi변경추가
        ChangeImageOnLoadCoroutine = StartCoroutine(ChangeImageOnLoad());
        TMIGeneratorCoroutine = StartCoroutine(TMIGenerator());
    }

    private void OnDisable()
    {
        StopCoroutine(ChangeImageOnLoadCoroutine);
        StopCoroutine(TMIGeneratorCoroutine);
    }

    Coroutine TMIGeneratorCoroutine;

    //TMI작성한 것들 랜덤하게 나오게 
    IEnumerator TMIGenerator()
    {
        int beforeValue = -1;
        int randomValue = -1;

        while (true)
        {
            randomValue = GetRandomValue(beforeValue, 1);
            beforeValue = randomValue;

            string tmiData = tmi[randomValue];

            tmiTMP.text = "";

            foreach (char each in tmiData)
            {
                tmiTMP.text += each;

                yield return new WaitForSeconds(tmiWordingDelay);
            }

            yield return new WaitForSeconds(tmiChangeDelay);

            for (int i = tmiTMP.text.Length - 1; i >= 6; i--)
            {
                tmiTMP.text = tmiTMP.text.Substring(0, i);

                yield return new WaitForSeconds(tmiWordingDelay);
            }
        }
    }

    Coroutine ChangeImageOnLoadCoroutine;

/// <summary>
/// 
/// </summary>
/// <param name="beforeValue"></param>
/// <param name="mode">1 -> TMI  2-> LoadImage</param>
/// <returns></returns>
    private int GetRandomValue(int beforeValue, int mode)
    {
        int randomValue = Random.Range(0, mode == 1 ? tmi.Length : mode == 2 ? loadImages.Length : 0);

        while (beforeValue == randomValue)
                randomValue = Random.Range(0, mode == 1 ? tmi.Length : mode == 2 ? loadImages.Length : 0);

        return randomValue;
    }

    IEnumerator ChangeImageOnLoad()
    {
        int beforeValue = -1;
        int randomValue = -1;

        while (true)
        {
            yield return new WaitForSeconds(imageChangeDelay); 

            Coroutine coroutine = StartCoroutine(FadeIn(fadeTimeDelay));;

            yield return coroutine;
            
            randomValue = GetRandomValue(beforeValue, 2);
            beforeValue = randomValue;
            
            Sprite sprite = loadImages[randomValue];

            loadImage.sprite = sprite;

            StartCoroutine(FadeOut(fadeTimeDelay)); 
            //yield return new WaitForSeconds(imageChangeDelay);
        }
    }

    IEnumerator FadeIn(float fadeTime)
    {
        loadImage.color = fadeColor;

        float time = fadeTime;

        while (true)
        {
            if ((time -= Time.deltaTime) <= 0.0f)
            {
                break;
            }

            Color color = new Color(1f, 1f, 1f, 1f);

            color *= time / fadeTime;
            loadImage.color = color;

            yield return null;
        }

        fadeColor.a = 0f;
    }

    IEnumerator FadeOut(float fadeTime)
    {
        loadImage.color = fadeColor;

        float time = 0.0f;

        while (true)
        {
            if ((time += Time.deltaTime) >= fadeTime)
            {
                break;
            }

            Color color = new Color(1f, 1f, 1f, 1f);

            color *= time / fadeTime;
            loadImage.color = color;

            yield return null;
        }

        fadeColor.a = 1f;
        loadImage.color = fadeColor;
    }

    private void LoadBarProgress(float progress)
    {
        loadBar.fillAmount = progress;
    }
}
