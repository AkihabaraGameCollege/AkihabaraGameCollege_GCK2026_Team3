using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// クリアシーンの管理クラス
    /// </summary>
    public class ClearScene : MonoBehaviour
    {
        /// <summary>
        /// BGMオーディオ設定の変数
        /// </summary>
        [SerializeField]
        private AudioSetting audioSettingBGM = null;
        /// <summary>
        /// SEオーディオ設定の変数
        /// </summary>
        [SerializeField]
        private AudioSetting audioSettingSE = null;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            audioSettingSE.PlaySE(1);
            audioSettingBGM.PlayBGM(4);
        }

        /// <summary>
        /// タイトルシーンへ遷移する関数
        /// </summary>
        public void InTitleScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
    }
}