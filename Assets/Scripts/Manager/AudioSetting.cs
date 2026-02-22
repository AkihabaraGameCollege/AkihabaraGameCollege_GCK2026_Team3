using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CardDefenseGame
{
    /// <summary>
    /// オーディオ設定管理クラス
    /// </summary>
    public class AudioSetting : MonoBehaviour
    {
        /// <summary>
        /// ソースオブジェクトの変数
        /// </summary>
        [SerializeField]
        public AudioSource bgmAudioSource;
        [SerializeField]
        public AudioSource seAudioSource;

        /// <summary>
        /// スライダーコンポーネントの変数
        /// </summary>
        private Slider bgmVolumeSlider;
        private Slider seVolumeSlider;

        /// <summary>
        /// リストオブジェクトの変数
        /// </summary>
        public List<AudioClip> bgms;
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
        /// 初期設定の関数
        /// </summary>
        void Awake()
        {
            // シーン内からSliderを探して取得
            bgmVolumeSlider = GameObject.Find("BgmVolumeSlider").GetComponent<Slider>();
            seVolumeSlider = GameObject.Find("SeVolumeSlider").GetComponent<Slider>();

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

                // スライダーの値が変更された時の処理を登録
                bgmVolumeSlider.onValueChanged.AddListener(ChangeVolumeBGM);
                seVolumeSlider.onValueChanged.AddListener(ChangeVolumeSE);
        }

        /// <summary>
        /// BGM音量変更の関数
        /// </summary>
        /// <param name="newVolume"></param>
        void ChangeVolumeBGM(float newVolume)
        {
            // スライダーの値によって音量を変更
            bgmAudioSource.volume = newVolume;
        }

        /// <summary>
        /// SE音量変更の関数
        /// </summary>
        /// <param name="newVolume"></param>
        void ChangeVolumeSE(float newVolume)
        {
            // スライダーの値によって音量を変更
            seAudioSource.volume = newVolume;
        }

        /// <summary>
        /// SEを鳴らすための関数
        /// </summary>
        /// <param name="seIndex"></param>
        void PlaySE(int seIndex)
        {
            seAudioSource.clip = ses[seIndex];
            seAudioSource.Play();
        }

        /// <summary>
        /// BGMを鳴らすための関数
        /// </summary>
        /// <param name="bgmIndex"></param>
        public void PlayBGM(int bgmIndex)
        {
            bgmAudioSource.clip = bgms[bgmIndex];
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
        public void StopSE()
        {
            seAudioSource.Stop();
        }

        /// <summary>
        /// 全てのオーディオを止めるための関数
        /// </summary>
        public void StopAllAudio()
        {
            bgmAudioSource.Stop();
            seAudioSource.Stop();
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
    }
}