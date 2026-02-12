using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardDefenseGame
{
    /// <summary>
    /// オーディオ設定管理クラス
    /// </summary>
    public class AudioSetting : MonoBehaviour
    {
        /// <summary>
        /// スライダーコンポーネントの変数
        /// </summary>
        private Slider bgmVolumeSlider;
        private Slider seVolumeSlider;

        /// <summary>
        /// ソースオブジェクトの変数
        /// </summary>
        public GameObject bgmAudioSource;
        public GameObject seAudioSource;

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
        /// 初期設定の関数
        /// </summary>
        void Start()
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
            bgmAudioSource.GetComponent<AudioSource>().volume = UpdateVolume.bgmSliderValue;
            seAudioSource.GetComponent<AudioSource>().volume = UpdateVolume.seSliderValue;

            // スライダーの値が変更された時の処理を登録
            bgmVolumeSlider.onValueChanged.AddListener(ChangeVolumeBGM);
            seVolumeSlider.onValueChanged.AddListener(ChangeVolumeSE);
        }

        /// <summary>
        /// BGM音量変更の関数
        /// </summary>
        /// <param name="newVolume"></param>
        private void ChangeVolumeBGM(float newVolume)
        {
            // スライダーの値によって音量を変更
            bgmAudioSource.GetComponent<AudioSource>().volume = newVolume;
        }

        /// <summary>
        /// SE音量変更の関数
        /// </summary>
        /// <param name="newVolume"></param>
        private void ChangeVolumeSE(float newVolume)
        {
            // スライダーの値によって音量を変更
            seAudioSource.GetComponent<AudioSource>().volume = newVolume;
        }

        /// <summary>
        /// SEを鳴らすための関数
        /// </summary>
        /// <param name="seIndex"></param>
        public void PlaySE(int seIndex)
        {
            seAudioSource.GetComponent<AudioSource>().clip = ses[seIndex];
            seAudioSource.GetComponent<AudioSource>().Play();
        }

        /// <summary>
        /// BGMを鳴らすための関数
        /// </summary>
        /// <param name="bgmIndex"></param>
        public void PlayBGM(int bgmIndex)
        {
            bgmAudioSource.GetComponent<AudioSource>().clip = bgms[bgmIndex];
            bgmAudioSource.GetComponent<AudioSource>().Play();
        }
    }
}