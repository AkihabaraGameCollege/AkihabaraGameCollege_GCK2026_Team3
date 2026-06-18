using System.Collections;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>     
    /// ポーズUIの管理を行うクラス     
    /// </summary>     
    public class SupportEffectUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// バフエフェクトのUIオブジェクトを参照する変数
        /// </summary>
        public GameObject buffEffectObject;
        /// <summary>
        /// 回復エフェクトのUIオブジェクトを参照する変数
        /// </summary>
        public GameObject heal_EffectObject;
        /// <summary>
        /// シールドエフェクトのUIオブジェクトを参照する変数
        /// </summary>
        public GameObject shieldEffectObject;

        /// <summary>
        /// バフエフェクトのUIのアニメーターを参照する変数
        /// </summary>
        public Animator buffAnimator;
        /// <summary>
        /// 回復エフェクトのUIのアニメーターを参照する変数
        /// </summary>
        public Animator heal_Animator;
        /// <summary>
        /// シールドエフェクトのUIのアニメーターを参照する変数
        /// </summary>
        public Animator shieldAnimator;

        /// <summary>
        /// サポートエフェクトUI管理クラスのインスタンスを参照する変数
        /// </summary>
        public static SupportEffectUI_Manager Instance { get; private set; }

        /// <summary>
        /// UIの表示・演出の時間
        /// </summary>
        private float animationTime = 2.0f;

        /// <summary>
        /// バフエフェクトのUIの番号を参照する変数
        /// </summary>
        public int buffEffectNumber = 0;
        /// <summary>
        /// 回復エフェクトのUIの番号を参照する変数
        /// </summary>
        public int heal_EffectNumber = 1;
        /// <summary>
        /// シールドエフェクトのUIの番号を参照する変数
        /// </summary>
        public int shieldEffectNumber = 2;
        /// <summary>
        /// バフ演出するときに呼ばれるIDを参照する変数
        /// </summary>
        private static readonly int buffTrigger = Animator.StringToHash("OnBuff");
        /// <summary>
        /// 回復演出するときに呼ばれるIDを参照する変数
        /// </summary>
        private static readonly int heal_Trigger = Animator.StringToHash("OnHeal");
        /// <summary>
        /// シールド演出するときに呼ばれるIDを参照する変数
        /// </summary>
        private static readonly int shieldTrigger = Animator.StringToHash("OnShield");

        /// <summary>         
        /// 初期設定を行う関数         
        /// </summary>         
        private void Awake()
        {
            Instance = this;
            Hide();// 起動時はUIを隠す
        }

        /// <summary>         
        /// UIを隠す関数         
        /// </summary>         
        private void Hide()
        {
            // 子オブジェクトをすべて非アクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>         
        /// 指定したUIの表示を行う関数         
        /// </summary>         
        private void TargetShow(GameObject target)
        {
            target.SetActive(true);// 指定したUIをアクティブ化
        }

        /// <summary>         
        /// 指定したUIの表示をやめる関数         
        /// </summary>         
        private void TargetHide(GameObject target)
        {
            target.SetActive(false);// 指定したUIを非アクティブ化
        }

        /// <summary>
        /// バフエフェクトのUIを表示するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowBuffEffectUI()
        {
            TargetShow(buffEffectObject);// 指定したUIを表示
            heal_Animator.SetTrigger(buffTrigger);// 指定したエフェクトのアニメーションを再生
            yield return new WaitForSeconds(animationTime);// 指定した時間待機
            TargetHide(buffEffectObject);// 指定したUIを非表示
        }

        /// <summary>
        /// 回復エフェクトのUIを表示するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowHeal_EffectUI()
        {
            TargetShow(heal_EffectObject);// 指定したUIを表示
            heal_Animator.SetTrigger(heal_Trigger);// 指定したエフェクトのアニメーションを再生
            yield return new WaitForSeconds(animationTime);// 指定した時間待機
            TargetHide(heal_EffectObject);// 指定したUIを非表示
        }

        /// <summary>
        /// シールドエフェクトのUIを表示するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowShieldEffectUI()
        {
            TargetShow(shieldEffectObject);// 指定したUIを表示
            heal_Animator.SetTrigger(shieldTrigger);// 指定したエフェクトのアニメーションを再生
            yield return new WaitForSeconds(animationTime);// 指定した時間待機
            TargetHide(shieldEffectObject);// 指定したUIを非表示
        }

        /// <summary>
        /// 指定したUIの表示・演出を行う関数
        /// </summary>
        /// <param name="targetNumber"></param>
        public void ShowSupportEffectUI(int targetNumber)
        {
            if (targetNumber == buffEffectNumber)
            {
                StartCoroutine(ShowBuffEffectUI());// バフエフェクトのUIを表示するコルーチンを開始
            }
            else if (targetNumber == heal_EffectNumber)
            {
                StartCoroutine(ShowHeal_EffectUI());// 回復エフェクトのUIを表示するコルーチンを開始
            }
            else if (targetNumber == shieldEffectNumber)
            {
                StartCoroutine(ShowShieldEffectUI());// シールドエフェクトのUIを表示するコルーチンを開始
            }
        }
    }
}