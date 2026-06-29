using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] public  AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip bgmClip;

    [Header("SFX Clips (optional — fill in Inspector)")]
    [SerializeField] private AudioClip crashClip;
    [SerializeField] private AudioClip finishClip;
    [SerializeField] private AudioClip trickClip;
    [SerializeField] private AudioClip jumpClip;

    private const string KeySFX   = "SFXVolume";
    private const string KeyMusic = "MusicVolume";

    private float _sfxVolume   = 1f;
    private float _musicVolume = 0.5f;

    public float SFXVolume
    {
        get => _sfxVolume;
        set { _sfxVolume = Mathf.Clamp01(value); ApplySFX();   PlayerPrefs.SetFloat(KeySFX,   _sfxVolume); }
    }

    public float MusicVolume
    {
        get => _musicVolume;
        set { _musicVolume = Mathf.Clamp01(value); ApplyMusic(); PlayerPrefs.SetFloat(KeyMusic, _musicVolume); }
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sfxVolume   = PlayerPrefs.GetFloat(KeySFX,   1f);
        _musicVolume = PlayerPrefs.GetFloat(KeyMusic, 0.5f);
        ApplySFX();
        ApplyMusic();

        if (musicSource != null && bgmClip != null)
        {
            musicSource.clip  = bgmClip;
            musicSource.loop  = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = -1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volume < 0 ? _sfxVolume : volume);
    }

    public void PlayCrashSound()        => PlaySFX(crashClip);
    public void PlayFinishSound()       => PlaySFX(finishClip);
    public void PlayTrickSuccessSound() => PlaySFX(trickClip);
    public void PlayJumpSound()         => PlaySFX(jumpClip);
    public void ResumeBoardingSound()   { /* no-op: BGM keeps playing */ }
    public void PauseBoardingSound()    { /* no-op: BGM keeps playing */ }

    void ApplySFX()   { if (sfxSource   != null) sfxSource.volume   = _sfxVolume;   }
    void ApplyMusic() { if (musicSource  != null) musicSource.volume = _musicVolume; }
}
