using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("オーディオミキサーの設定")]
    [SerializeField]
    private AudioMixer audioMixer; // オーディオミキサー

    [Header("BGMのAudioSource")]
    [SerializeField]
    private AudioSource bgmAudioSource; // BGMのAudioSource

    [Header("歩行時のSE")]
    [SerializeField]
    private AudioSource walkAudioSource; // 歩行時の効果音

    [Header("スプリント時のSE")]
    [SerializeField]
    private AudioSource sprintAudioSource; // スプリント時の効果音

    [Header("チェックポイントのSE")]
    [SerializeField]
    private AudioSource checkpointAudioSource; // チェックポイントの効果音

    [Header("ゲームクリアのSE")]
    [SerializeField]
    private AudioSource gameClearAudioSource; // ゲームクリアの効果音

    [Header("ゲームオーバーのSE")]
    [SerializeField]
    private AudioSource gameOverAudioSource; // ゲームオーバーの効果音

    [Header("クリック音のSE")]
    [SerializeField]
    private AudioSource clickAudioSource; // クリック音の効果音

    [Header("テレポートのSE")]
    [SerializeField]
    private AudioSource teleportAudioSource; // テレポートの効果音

    [Header("操作反転のSE")]
    [SerializeField]
    private AudioSource reverseControlAudioSource; // 操作反転の効果音

    [Header("ジャンプ時のSE")]
    [SerializeField]
    private AudioSource jumpAudioSource; // ジャンプ時の効果音

    [Header("炎ダメージのSE")]
    [SerializeField]
    private AudioSource fireDamageAudioSource; // 炎ダメージの効果音

    [Header("マスター音量スライダー")]
    [SerializeField]
    private Slider masterVolumeSlider; // マスター音量スライダー

    [Header("SE音量スライダー")]
    [SerializeField]
    private Slider seVolumeSlider; // 効果音音量スライダー

    [Header("BGM音量スライダー")]
    [SerializeField]
    private Slider bgmVolumeSlider; // BGM音量スライダー

    private void Start()
    {
        // 各AudioSourceの初期設定
        SetupAudioSource(walkAudioSource);
        SetupAudioSource(sprintAudioSource);
        SetupAudioSource(bgmAudioSource);
        SetupAudioSource(checkpointAudioSource);
        SetupAudioSource(gameClearAudioSource);
        SetupAudioSource(gameOverAudioSource);
        SetupAudioSource(clickAudioSource);
        SetupAudioSource(teleportAudioSource);
        SetupAudioSource(reverseControlAudioSource);
        SetupAudioSource(jumpAudioSource);
        SetupAudioSource(fireDamageAudioSource);

        // マスター音量スライダーの設定
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }

        // 効果音音量スライダーの設定
        if (seVolumeSlider != null)
        {
            seVolumeSlider.onValueChanged.AddListener(SetSEVolume);
            seVolumeSlider.value = PlayerPrefs.GetFloat("SEVolume", 1f);
        }

        // BGM音量スライダーの設定
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
            bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        }

        // シーンをまたいで音量設定を適用
        ApplySavedVolumes();
    }

    private void ApplySavedVolumes()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(masterVolume);

        float seVolume = PlayerPrefs.GetFloat("SEVolume", 1f);
        SetSEVolume(seVolume);

        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        SetBGMVolume(bgmVolume);
    }

    // 歩行時の効果音を再生
    public void PlayWalkAudio()
    {
        if (!walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
    }

    // 歩行時の効果音を停止
    public void StopWalkAudio()
    {
        walkAudioSource.Stop();
    }

    // スプリント時の効果音を再生
    public void PlaySprintAudio()
    {
        if (!sprintAudioSource.isPlaying)
        {
            sprintAudioSource.Play();
        }
    }

    // スプリント時の効果音を停止
    public void StopSprintAudio()
    {
        sprintAudioSource.Stop();
    }

    // クリア時のSEを再生
    public void PlayGameClearAudio()
    {
        if (gameClearAudioSource != null)
        {
            gameClearAudioSource.Play();
        }
    }

    // ゲームオーバーの効果音を再生
    public void PlayGameOverAudio()
    {
        if (gameOverAudioSource != null)
        {
            gameOverAudioSource.Play();
        }
    }

    // BGMを停止
    public void StopbgmAudio()
    {
        bgmAudioSource.Stop();
    }

    // クリック音を再生
    public void PlayClickAudio()
    {
        if (clickAudioSource != null)
        {
            clickAudioSource.Play();
        }
    }

    // ジャンプ時の効果音を再生
    public void PlayJumpAudio()
    {
        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }
    }

    // 炎ダメージの効果音を再生
    public void PlayFireDamageAudio()
    {
        if (fireDamageAudioSource != null)
        {
            fireDamageAudioSource.Play();
        }
    }

    // BGM音量を設定
    public void SetBGMVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat("BGMVolume", dbVolume);
            PlayerPrefs.SetFloat("BGMVolume", volume);
        }
    }

    // 効果音音量を設定
    public void SetSEVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat("SEVolume", dbVolume);
            PlayerPrefs.SetFloat("SEVolume", volume);
        }
    }

    // マスター音量を設定
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat("MasterVolume", dbVolume);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }

    // チェックポイントの効果音を再生
    public void PlayCheckpointAudio()
    {
        if (checkpointAudioSource != null)
        {
            checkpointAudioSource.Play();
        }
    }

    // テレポートの効果音を再生
    public void PlayTeleportAudio()
    {
        if (teleportAudioSource != null)
        {
            teleportAudioSource.Play();
        }
    }

    // 操作反転の効果音を再生
    public void PlayReverseControlAudio()
    {
        if (reverseControlAudioSource != null)
        {
            reverseControlAudioSource.Play();
        }
    }

    // AudioSourceの初期設定
    private void SetupAudioSource(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            var seGroups = audioMixer.FindMatchingGroups("SEGroup");
            if (seGroups.Length > 0)
            {
                audioSource.outputAudioMixerGroup = seGroups[0];
            }
        }
    }

    // ゲーム終了時に呼ばれるメソッド
    private void OnApplicationQuit()
    {
        ResetSoundSettings();
    }

    // サウンド設定をリセットするメソッド
    private void ResetSoundSettings()
    {
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("SEVolume");
        PlayerPrefs.DeleteKey("BGMVolume");
    }
}
