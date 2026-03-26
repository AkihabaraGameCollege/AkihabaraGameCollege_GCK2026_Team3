using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.Universal;

namespace ForestDraw
{
    /// <summary>
    /// オーディオ設定管理クラス
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
        /// 開いているかどうかのフラグ
        /// </summary>
        public bool isOpening = false;
        /// <summary>
        /// フェード中かどうかのフラグ
        /// </summary>
        public bool isFading = false;
        /// <summary>
        /// サウンド管理クラスのインスタンスを参照する変数
        /// </summary>
        public static AudioSetting Instance { get; private set; } = null;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        void Awake()
        {
            Instance = this;
            // 保存された音量を反映
            bgmVolumeSlider.value = UpdateVolume.bgmSliderValue;
            seVolumeSlider.value = UpdateVolume.seSliderValue;
        }

        /// <summary>
        /// 毎フレーム更新の関数
        /// </summary>
        void Update()
        {
            // スライダーの値を取得
            UpdateVolume.bgmSliderValue = bgmVolumeSlider.value;
            UpdateVolume.seSliderValue = seVolumeSlider.value;

            // オーディオの音量を設定
            bgmAudioSource.volume = UpdateVolume.bgmSliderValue;
            seAudioSource.volume = UpdateVolume.seSliderValue;
            bgsAudioSource.volume = UpdateVolume.seSliderValue;

            // スライダーの値が変更されたときに呼び出される関数を登録
            bgmVolumeSlider.onValueChanged.AddListener(ChangeVolumeBGM);// BGM音量スライダーの値が変更されたときに呼び出される関数を登録
            seVolumeSlider.onValueChanged.AddListener(ChangeVolumeSE);// SE音量スライダーの値が変更されたときに呼び出される関数を登録
        }

        /// <summary>
        /// BGM音量変更の関数
        /// </summary>
        /// <param name="newVolume"></param>
        void ChangeVolumeBGM(float newVolume)
        {
            bgmAudioSource.volume = newVolume;
        }

        /// <summary>
        /// SE音量変更の関数
        /// </summary>
        /// <param name="newVolume"></param>
        void ChangeVolumeSE(float newVolume)
        {
            seAudioSource.volume = newVolume;
            bgsAudioSource.volume = newVolume;
        }

        /// <summary>
        /// SEを鳴らすための関数
        /// </summary>
        /// <param name="seIndex"></param>
        public void PlaySE(int seIndex)
        {
                seAudioSource.clip = ses[seIndex];// SEのオーディオクリップを設定
                seAudioSource.Play();
        }
        public void CardSE(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("カード使用時のSEがない");
                return;
            }
            seAudioSource.clip = clip;  
            seAudioSource.Play();
        }

        /// <summary>
        /// BGMを鳴らすための関数
        /// </summary>
        /// <param name="bgmIndex"></param>
        public void PlayBGM(int bgmIndex)
        {
            bgmAudioSource.clip = bgms[bgmIndex];// BGMのオーディオクリップを設定
            bgmAudioSource.Play();
        }

        /// <summary>
        /// BGMを止めるための関数
        /// </summary>
        public void StopBGM()
        {
            bgmAudioSource.Stop();
        }

        /// <summary>
        /// SEを止めるための関数
        /// </summary>
        public void StopBGS()
        {
            bgsAudioSource.Stop();
        }

        /// <summary>
        /// 全てのオーディオを止めるための関数
        /// </summary>
        public void StopAllAudio()
        {
            bgmAudioSource.Stop();
            seAudioSource.Stop();
            bgsAudioSource.Stop();
        }

        /// <summary>
        /// フェードアウトの関数
        /// </summary>
        /// <param name="time"></param>
        public void StopFadeOut(float time)
        {
            StartCoroutine(FadeOutCoroutine(time));
        }

        /// <summary>
        /// BGMフェードアウトのコルーチン
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        IEnumerator FadeOutCoroutine(float time)
        {
            float startVolume = bgmAudioSource.volume;// 現在の音量を保存
            isFading = true;// フェード中フラグを立てる

            // 音量が0になるまで徐々に減少させる
            while (bgmAudioSource.volume > 0)
            {
                bgmAudioSource.volume -= startVolume * Time.deltaTime / time;// 音量を減少させる
                yield return null;
            }

            bgmAudioSource.Stop();// BGMを停止
            isFading = false;// フェード中フラグを下ろす
            bgmAudioSource.volume = startVolume;// 音量を元に戻す
        }

        /// <summary>
        /// フェードインの関数
        /// </summary>
        /// <param name="time"></param>
        public void StartFadeIn(float time)
        {
            StartCoroutine(FadeInCoroutine(time));
        }

        /// <summary>
        /// BGMフェードインのコルーチン
        /// </summary>
        /// <returns></returns>
        IEnumerator FadeInCoroutine(float time)
        {
            float targetVolume = UpdateVolume.bgmSliderValue;// 目標の音量を保存
            isFading = true;// フェード中フラグを立てる
            bgmAudioSource.volume = 0;// 音量を0に設定
            bgmAudioSource.Play();// BGMを再生

            // 音量が目標の音量になるまで徐々に増加させる
            while (bgmAudioSource.volume < targetVolume)
            {
                bgmAudioSource.volume += targetVolume * Time.deltaTime / time;// 音量を増加させる
                yield return null;
            }

            isFading = false;// フェード中フラグを下ろす
            bgmAudioSource.volume = targetVolume;// 音量を目標の音量に設定
        }

        /// <summary>
        /// BGSを鳴らすための関数
        /// </summary>
        /// <param name="bgsIndex"></param>
        public void PlayBGS(int bgsIndex)
        {
            bgsAudioSource.clip = ses[bgsIndex];// BGSのオーディオクリップを設定
            bgsAudioSource.Play();
        }
    }
}