using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// オーディオ設定管理クラス（AudioMixerベース）
    /// </summary>
    public class AudioSetting : MonoBehaviour
    {
        /// <summary>
        /// BGM用のオーディオソースの変数
        /// </summary>
        public AudioSource bgmAudioSource = null;
        /// <summary>
        /// SE用のオーディオソースの変数
        /// </summary>
        public AudioSource seAudioSource = null;
        /// <summary>
        /// BGS用のオーディオソースの変数
        /// </summary>
        public AudioSource bgsAudioSource = null;

        /// <summary>
        /// BGM音量のスライダーの変数
        /// </summary>
        [SerializeField]
        private Slider bgmVolumeSlider = null;
        /// <summary>
        /// SE音量のスライダーの変数
        /// </summary>
        [SerializeField]
        private Slider seVolumeSlider = null;

        /// <summary>
        /// BGMのオーディオクリップのリスト変数
        /// </summary>
        public List<AudioClip> bgms;
        /// <summary>
        /// SEのオーディオクリップのリスト変数
        /// </summary>
        public List<AudioClip> ses;

        /// <summary>
        /// サウンド管理クラスのインスタンスを参照する変数
        /// </summary>
        public static AudioSetting Instance { get; private set; }

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // スライダーの初期値を1にする（保存値があればそれを優先）
            if (bgmVolumeSlider != null)
            {
                float savedBgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
                bgmVolumeSlider.value = savedBgm;
                UpdateVolume.bgmSliderValue = savedBgm;
                bgmVolumeSlider.onValueChanged.AddListener(ChangeVolumeBGM);
                // 初期反映
                ChangeVolumeBGM(bgmVolumeSlider.value);
            }

            if (seVolumeSlider != null)
            {
                float savedSe = PlayerPrefs.GetFloat("SEVolume", 1f);
                seVolumeSlider.value = savedSe;
                UpdateVolume.seSliderValue = savedSe;
                seVolumeSlider.onValueChanged.AddListener(ChangeVolumeSE);
                // 初期反映
                ChangeVolumeSE(seVolumeSlider.value);
            }
        }

        /// <summary>
        /// 毎フレーム更新の関数
        /// </summary>
        void Update()
        {
            // スライダーの値を取得して共有フィールドへ反映（既存の仕組みとの互換）
            if (bgmVolumeSlider != null)
            {
                UpdateVolume.bgmSliderValue = bgmVolumeSlider.value;
            }

            if (seVolumeSlider != null)
            {
                UpdateVolume.seSliderValue = seVolumeSlider.value;
            }
        }

        /// <summary>
        /// BGM音量変更の関数（スライダーのコールバック）
        /// </summary>
        /// <param name="newVolume"></param>
        void ChangeVolumeBGM(float newVolume)
        {
            // UpdateVolumeとの同期（既存コード互換）
            UpdateVolume.bgmSliderValue = newVolume;


            // ミキサーが無ければソースボリュームで代替
            if (bgmAudioSource != null) bgmAudioSource.volume = newVolume;
        }

        /// <summary>
        /// SE音量変更の関数（スライダーのコールバック）
        /// </summary>
        /// <param name="newVolume"></param>
        void ChangeVolumeSE(float newVolume)
        {
            // UpdateVolumeとの同期（既存コード互換）
            UpdateVolume.seSliderValue = newVolume;

            if (seAudioSource != null) seAudioSource.volume = newVolume;
            if (bgsAudioSource != null) bgsAudioSource.volume = newVolume;
        }

        /// <summary>
        /// SEを鳴らすための関数
        /// </summary>
        /// <param name="seIndex"></param>
        public void PlaySE(int seIndex)
        {
            if (ses == null || seIndex < 0 || seIndex >= ses.Count)
            {
                Debug.LogError("SEインデックスが不正またはSEリストがありません: " + seIndex);
                return;
            }

            if (seAudioSource == null)
            {
                Debug.LogError("seAudioSource が設定されていません");
                return;
            }

            // PlayOneShotを使うことで音が重なるように再生
            seAudioSource.PlayOneShot(ses[seIndex]);
        }
        public void CardSE(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("カード使用時のSEがない");
                return;
            }
            if (seAudioSource == null)
            {
                Debug.LogError("seAudioSource が設定されていません");
                return;
            }

            // PlayOneShotを使うことで音が重なるように再生
            seAudioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// BGMを鳴らすための関数
        /// </summary>
        /// <param name="bgmIndex"></param>
        public void PlayBGM(int bgmIndex)
        {
            if (bgms == null || bgmIndex < 0 || bgmIndex >= bgms.Count)
            {
                Debug.LogError("BGMインデックスが不正またはBGMリストがありません: " + bgmIndex);
                return;
            }

            if (bgmAudioSource == null)
            {
                Debug.LogError("bgmAudioSource が設定されていません");
                return;
            }

            bgmAudioSource.clip = bgms[bgmIndex];
            bgmAudioSource.Play();
        }

        /// <summary>
        /// BGMを止めるための関数
        /// </summary>
        public void StopBGM()
        {
            if (bgmAudioSource != null) bgmAudioSource.Stop();
        }

        /// <summary>
        /// SEを止めるための関数
        /// </summary>
        public void StopBGS()
        {
            if (bgsAudioSource != null) bgsAudioSource.Stop();
        }

        /// <summary>
        /// 全てのオーディオを止めるための関数
        /// </summary>
        public void StopAllAudio()
        {
            if (bgmAudioSource != null) bgmAudioSource.Stop();
            if (seAudioSource != null) seAudioSource.Stop();
            if (bgsAudioSource != null) bgsAudioSource.Stop();
        }

        /// <summary>
        /// BGSを鳴らすための関数
        /// </summary>
        /// <param name="bgsIndex"></param>
        public void PlayBGS(int bgsIndex)
        {
            if (ses == null || bgsIndex < 0 || bgsIndex >= ses.Count)
            {
                Debug.LogError("BGSインデックスが不正またはSEリストがありません: " + bgsIndex);
                return;
            }

            if (bgsAudioSource == null)
            {
                Debug.LogError("bgsAudioSource が設定されていません");
                return;
            }

            bgsAudioSource.clip = ses[bgsIndex];
            bgsAudioSource.Play();
        }
    }
}