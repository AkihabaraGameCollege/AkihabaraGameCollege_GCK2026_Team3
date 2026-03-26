using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// 特定の敵出現時にプレイヤーに通知するクラス
    /// </summary>
    public class EnemyEncountNotice : MonoBehaviour
    {
        /// <summary>
        /// アニメーターコンポーネントを参照する変数
        /// </summary>
        private Animator animator = null;

        /// <summary>
        /// 通知UIのイメージを参照する変数
        /// </summary>
        private Image notice_Image = null;

        /// <summary>
        /// 通知UIのスプライトを参照するリスト変数
        /// </summary>
        public Sprite[] notice_Sprite = null;

        /// <summary>
        /// 特定の敵出現するときに呼ばれるIDを参照する変数
        /// </summary>
        private static readonly int enemyEncountTrigger = Animator.StringToHash("EnemyEncount");

        /// <summary>
        /// 敵出現通知UIのオブジェクト名を参照する変数
        /// </summary>
        private string noticeName = "EnemyEncount";
        /// <summary>
        /// ゴーレムのエネミー名を参照する変数
        /// </summary>
        private string golemName = "Golem";
        /// <summary>
        /// ウィザードのエネミー名を参照する変数
        /// </summary>
        private string wizardName = "Wizard";
        /// <summary>
        /// トレントのエネミー名を参照する変数
        /// </summary>
        private string treantName = "Treant";

        /// <summary>
        /// スプライトのリストを指定する数を参照する変数
        /// </summary>
        private int sprite_Index = 0;
        /// <summary>
        /// ゴーレムの通知スプライトを指定する数を参照する変数
        /// </summary>
        private int golemSprite_Index = 0;
        /// <summary>
        /// ウィザードの通知スプライトを指定する数を参照する変数
        /// </summary>
        private int wizardSprite_Index = 1;
        /// <summary>
        /// トレントの通知スプライトを指定する数を参照する変数
        /// </summary>
        private int treantSprite_Index = 2;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // コンポーネントの登録
            animator = GetComponent<Animator>();// アニメーターを取得
            notice_Image = GameObject.Find(noticeName).GetComponent<Image>();// シーン内から通知UIを探して取得
        }

        /// <summary>
        /// 出現した敵を通知する関数
        /// </summary>
        /// <param name="name"></param>
        public void NoticeEnemyEncount(string name)
        {
            if (name == golemName)
            {
                sprite_Index = golemSprite_Index;// スプライトのインデックスにゴーレムの情報を代入
            }
            else if (name == wizardName)
            {
                sprite_Index = wizardSprite_Index;// スプライトのインデックスにウィザードの情報を代入
            }
            else if (name == treantName)
            {
                sprite_Index = treantSprite_Index;// スプライトのインデックスにトレントの情報を代入
            }
            else
            {
                return;// どの敵の名前でもないのなら通知しない
            }

            notice_Image.sprite = notice_Sprite[sprite_Index];// 通知UIのスプライトをステージに合わせて変更
            animator.SetTrigger(enemyEncountTrigger);// 通知するトリガーをセット
        }
    }
}