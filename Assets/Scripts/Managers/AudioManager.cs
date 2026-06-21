using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Singleton audio controller.
/// Exposes SFXVolume and MusicVolume (0-1); settings persisted via PlayerPrefs.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────
    public static AudioManager Instance { get; private set; }

    // ── Inspector ──────────────────────────────────────────────
    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip bgmClip;

    // ── Constants ──────────────────────────────────────────────
    private const string KeySFX   = "SFXVolume";
    private const string KeyMusic = "MusicVolume";

    // ── Properties ─────────────────────────────────────────────
    private float _sfxVolume   = 1f;
    private float _musicVolume = 0.5f;

    public float SFXVolume
    {
        get => _sfxVolume;
        set { _sfxVolume = Mathf.Clamp01(value); ApplySFX();   PlayerPrefs.SetFloat(KeySFX,   _sfxVolume);   }
    }

    public float MusicVolume
    {
        get => _musicVolume;
        set { _musicVolume = Mathf.Clamp01(value); ApplyMusic(); PlayerPrefs.SetFloat(KeyMusic, _musicVolume); }
    }

    // ── Unity Lifecycle ────────────────────────────────────────
    private void Awake()
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
            musicSource.clip   = bgmClip;
            musicSource.loop   = true;
            musicSource.Play();
        }
    }

    // ── Public API ─────────────────────────────────────────────
    /// <summary>Play a one-shot SFX clip at the specified volume (default: SFXVolume).</summary>
    public void PlaySFX(AudioClip clip, float volume = -1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volume < 0 ? _sfxVolume : volume);
    }

    // ── Helpers ────────────────────────────────────────────────
    private void ApplySFX()
    {
        if (sfxSource != null) sfxSource.volume = _sfxVolume;
    }

    private void ApplyMusic()
    {
        if (musicSource != null) musicSource.volume = _musicVolume;
    }
}
