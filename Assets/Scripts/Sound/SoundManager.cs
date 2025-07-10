using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : ObjectPooling<SoundManager, Sound>
{
    public enum BGM
    {
        BGM_GeuNaSa,
        BGM_MaSaeDol,
        BGM_Sukidakara,
        //위 음원들은 추후 삭제 예정
        BGM_WakEnt_1,
        BGM_WakEnt_2,
        BGM_WakEnt_3,
        BGM_Processing_1,
        BGM_Main_1,
        BGM_Event_Positive,
    }

    public enum Effect
    {
        Effect_GoSeGu_KingA,
        Effect_GoSeGu_Muyo,
        Effect_Jururu_HuHeEng,
        Effect_ReviewSetUnder5,
        Effect_ReviewSetOver5,
        Effect_ReviewSet10,
        Effect_ReviewTotalPoint,
        Effect_ReviewTotalPointFixedContent,
        Effect_FieldStatPopup,
        Effect_UIPopupMessage,

    }
    
    public interface OnEndBGM
    {
        public void SetEndBGMEvent(OnEndBGMEvent OnEndBGMEvent);
    }

    public interface OnChangeVol
    {
        public void SetOnChangeVol(OnChangeVolEvent OnChangeVolEvent);
    }

    public interface RegistrationSound
    {
        public void SetRegistrationSound(OnRegistrationSound OnRegistrationSound);
    }

    public delegate void OnRegistrationSound(Sound sound);
    public delegate float OnChangeVolEvent(SoundType soundType);

    public delegate void OnEndBGMEvent(AudioSource audioSource);


    public enum SoundType
    {
        Master,
        Bgm,
        Effect,
    }

    public float masterVol { get; private set; }
    public float bgmVol { get; private set; }
    public float effectVol { get; private set;}

    private const string KEY_MASTER = "MasterVol";
    private const string KEY_BGM = "BgmVol";
    private const string KEY_EFFECT = "EffectVol";

    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip[] effects;

    [Header("Source")]
    [SerializeField] private AudioSource nowPlaySource;
    [SerializeField] private List<Sound> playSoundList;

    [SerializeField] private AudioMixer mixer;

    private new void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);

        playSoundList = new List<Sound>();
        nowPlaySource = gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //TestGetSound(SoundType.Effect);

        //bgmSource.clip = Resources.Load("BGM_Sukidakara",typeof(AudioClip)) as AudioClip;

        //encrypt넣어서 할 듯
        masterVol = PlayerPrefs.HasKey(KEY_MASTER) ? PlayerPrefs.GetFloat(KEY_MASTER) : 1.0f;
        bgmVol = PlayerPrefs.HasKey(KEY_BGM) ? PlayerPrefs.GetFloat(KEY_BGM) : 1.0f;
        effectVol = PlayerPrefs.HasKey(KEY_EFFECT) ? PlayerPrefs.GetFloat(KEY_EFFECT) : 1.0f;

        //bgmSource.volume = bgmVol * masterVol;

        //bgmSource.Play();
    }

    private new void Start()
    {
        base.Start();

        PlaySound(BGM.BGM_Main_1.ToString(), false);    
    }

    public void OnChangedVol(SoundType type, float value)
    {
        switch (type)
        {
            case SoundType.Master:
            masterVol = value;
            PlayerPrefs.SetFloat(KEY_MASTER, value);
            break;
            case SoundType.Bgm:
            bgmVol = value;
            PlayerPrefs.SetFloat(KEY_BGM, value);
            break;
            case SoundType.Effect:
            effectVol = value;
            PlayerPrefs.SetFloat(KEY_EFFECT, value);
            break;
        }

        foreach (Sound sound in playSoundList)
        {
            sound.SetVol();
        }

        nowPlaySource.volume = masterVol * bgmVol;
    }

    public void RegistrationSoundComponent(Sound sound)
    {
        playSoundList.Add(sound);
    }

    public float VolChangeEvent(SoundType type)
    {
        float value = SoundType.Effect == type ? effectVol : bgmVol;

        return masterVol * value;
    }

    public AudioClip GetClip(SoundType type, string name, bool multiBGM)
    {
        AudioClip[] clips = (type == SoundType.Bgm) ? bgms : effects;

        foreach (AudioClip clip in clips)
        {
            if (clip.name == name)
            {
                if (type == SoundType.Bgm)
                {
                    PauseBGM();

                    //playsoundlist 비워야 됨
                    if (!multiBGM)
                    {
                        nowPlaySource.clip = null;
                    }
                }

                return clip;
            }
        }

        return null;
    }

    // public void PlayBGM(string name)
    // {
    //     nowPlaySource.clip = GetClip(SoundType.Bgm, name);

    //     nowPlaySource.Play();
    // }

    public void PauseBGM()
    {
        nowPlaySource.Pause();
    }

    public void UnPauseBGM()
    {
        nowPlaySource.UnPause();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="multiBGM">작업자 작업 시 작업 배경음을 틀고 </param>
    public void PlaySound(string name, bool isMainBGM, bool multiBGM = false)
    {
        if (Instance == null)
        {
            return;
        }

        if (playSoundList == null)
        {
            return;
        }

        Sound sound = GetPool();

        string[] soundType = name.Split('_');
        playSoundList.Add(sound);

        SoundType type = soundType[0] == "Effect" ? SoundType.Effect : SoundType.Bgm;

        sound.Play(GetClip(type, name, multiBGM), masterVol * effectVol, type, isMainBGM, multiBGM);

        nowPlaySource.volume = masterVol * bgmVol;

        AudioSource source = sound.GetAudioSource();
        source.loop = false;

        if (multiBGM)
        {
            nowPlaySource?.Pause();

            this.multiBGM = source;
        }

        if (soundType[0] == "BGM")
        {   
            source.loop = true;
        }
    }

    private AudioSource multiBGM;

    public void EndBGM(AudioSource audioSource)
    {
        nowPlaySource = audioSource;
    }

    //배경음 대신에 따로 효과용으로 BGM을 틀경우 기존 BGM을 잠시 멈춤

    public void EndMultiAudio()
    {
        multiBGM?.Pause();
    }
    public void ReplayAudio()
    {
        if (nowPlaySource != null)
        {
            nowPlaySource.gameObject.SetActive(true);
        }

        nowPlaySource?.UnPause();
    }

    public string[] TestGetSound(SoundType type)
    {
        string[] audioClips;
        int count = type == SoundType.Effect ? effects.Length : bgms.Length;

        audioClips = type == SoundType.Effect ? new string[effects.Length] : new string[bgms.Length];

        for (int i = 0; i < count; i++)
        {
            audioClips[i] = type == SoundType.Effect ? effects[i].name : bgms[i].name;
        }

        return audioClips;
    }
}
