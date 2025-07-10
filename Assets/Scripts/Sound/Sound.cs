using System.Collections;
using UnityEngine;

public class Sound : MonoBehaviour, OnReturnPool<Sound>, 
                                    SoundManager.OnEndBGM, 
                                    SoundManager.OnChangeVol, 
                                    SoundManager.RegistrationSound
{
    private OnReturnPoolEvent<Sound> OnReturnPool;
    private SoundManager.OnEndBGMEvent OnEndBGMEvent;

    private SoundManager.OnChangeVolEvent OnChangeVolEvent;
    private SoundManager.OnRegistrationSound OnRegistrationSound;
    private AudioSource audioSource;

    SoundManager.SoundType type;
    bool isMainBGM;

    public void Play(AudioClip clip, float vol, SoundManager.SoundType type, bool isMainBGM, bool playNoOffBGM)
    {
        audioSource.clip = clip;
        audioSource.volume = vol;
        audioSource.Play();
        this.type = type;
        this.isMainBGM = isMainBGM;

        StartCoroutine(Playing(playNoOffBGM));
    }

    private IEnumerator Playing(bool multiBGM)
    {
        string[] names = audioSource.clip.name.Split("_");

        if (!multiBGM && names[0] == "BGM")
        {
            OnEndBGMEvent(audioSource);
        }

        while (isMainBGM || audioSource.isPlaying && audioSource.clip != null)
        {
            yield return null;
        }

        if (multiBGM)
        {
            SoundManager.Instance.ReplayAudio();
        }

        OnReturnPool?.Invoke(this);
    }

    public void TestOnReturn()
    {
        OnReturnPool?.Invoke(this);
    }

    public void SetVol()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        float vol = OnChangeVolEvent(type);

        audioSource.volume = vol;
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }

    public void Init(OnReturnPoolEvent<Sound> OnReturnPool)
    {
        this.OnReturnPool = OnReturnPool;
        
        audioSource = GetComponent<AudioSource>();

        SetEndBGMEvent(SoundManager.Instance.EndBGM);
        SetOnChangeVol(SoundManager.Instance.VolChangeEvent);
        SetRegistrationSound(SoundManager.Instance.RegistrationSoundComponent);

        OnRegistrationSound(this);
    }

    public void SetEndBGMEvent(SoundManager.OnEndBGMEvent OnEndBGMEvent)
    {
        this.OnEndBGMEvent = OnEndBGMEvent;
    }

    public void SetOnChangeVol(SoundManager.OnChangeVolEvent OnChangeVolEvent)
    {
        this.OnChangeVolEvent = OnChangeVolEvent;
    }

    public void SetRegistrationSound(SoundManager.OnRegistrationSound OnRegistrationSound)
    {
        this.OnRegistrationSound = OnRegistrationSound;
    }
}
