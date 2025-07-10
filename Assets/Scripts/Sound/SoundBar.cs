using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class SoundBar : MonoBehaviour
{
    //이거를 사운드 바에 하나 씩 붙혀주고 사운드 바가 움직일 때 SoundManager의 changevalue 불러주면 될 듯
    //값이랑 사운드 타입을 여기서 soundManager로 보내주는 구조

    private Slider slider;
    [SerializeField] private SoundManager.SoundType type;

    [SerializeField] private TextMeshProUGUI text;

    void Awake()
    {   
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        slider.value =  type == SoundManager.SoundType.Master ? SoundManager.Instance.masterVol 
                                : type == SoundManager.SoundType.Effect ? SoundManager.Instance.effectVol
                                : SoundManager.Instance.bgmVol;
    }

    /// <summary>
    /// type은 SoundManager의 SoundType을 따름
    /// </summary>
    /// <param name="type">Master, Effect, BGM</param>
    public void RequestChangeVol()
    {
        // if (this.type == SoundManager.SoundType.Default)
        // {
        //     this.type = type == SoundManager.SoundType.Master.ToString() ? SoundManager.SoundType.Master
        //                          : type == SoundManager.SoundType.Effect.ToString() ? SoundManager.SoundType.Effect 
        //                          : SoundManager.SoundType.Bgm;
        // }
        SoundManager.Instance.OnChangedVol(type, slider.value);

        text.SetText(string.Format("{0:N0}", slider.value * 100f));
    }
}
