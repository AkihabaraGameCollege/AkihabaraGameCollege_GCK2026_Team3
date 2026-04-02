using UnityEngine;
using UnityEngine.InputSystem;

namespace ForestDraw
{
    /// <summary>
    /// プレイヤーの操作関係のクラス
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// カメラの参照用変数
        /// </summary>
        private Transform mainCamera = null;

        /// <summary>
        /// ポーズクラスの参照変数
        /// </summary>
        public PauseUI_Manager pauseManager = null;

        /// <summary>
        /// 左右の最大回転角度の参照変数
        /// </summary>
        public float maxAngle = 30f;
        /// <summary>
        /// カメラが回転する速さの参照変数
        /// </summary>
        public float rotationSpeed = 90f;
        /// <summary>
        /// カメラの回転軸Yの参照変数
        /// </summary>
        private float currentYAngle = 0f;
        /// <summary>
        /// キー入力時の状態を管理する変数
        /// </summary>
        private float inputHorizontal = 0;

        /// <summary>
        /// カメラの初期回転状態を保持するための変数
        /// </summary>
        private Quaternion initialRotation;

        /// <summary>
        /// ポーズ操作の許可禁止を判別する変数
        /// </summary>
        public bool isCanPause = false;
        /// <summary>
        /// ポーズをするかどうか判別する変数
        /// </summary>
        private bool isPauseON = false;

        /// <summary>
        /// ポーズUIのオブジェクト名を参照する変数
        /// </summary>
        public string mainCameraName = "Main Camera";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            mainCamera = GameObject.Find(mainCameraName).GetComponent<Transform>();
            initialRotation = mainCamera.transform.localRotation;// カメラを初期回転状態にする
        }

        /// <summary>
        /// 毎フレーム処理の関数
        /// </summary>
        private void Update()
        {
            // 回転させるための準備を行う
            currentYAngle += inputHorizontal * rotationSpeed * Time.deltaTime;// Y軸回転を行う
            currentYAngle = Mathf.Clamp(currentYAngle, -maxAngle, maxAngle);// 角度を制限する

            mainCamera.localRotation = initialRotation * Quaternion.Euler(0f, currentYAngle, 0f);// カメラの回転させる
        }

        /// <summary>
        /// ポーズの入力判定を行う関数
        /// </summary>
        /// <param name="context"></param>
        public void OnPause(InputAction.CallbackContext context)
        {
            // もし入力された場合
            if (context.started && isCanPause)
            {
                pauseManager.StartCoroutine(pauseManager.PauseAnimationCoroutine(isPauseON));// ポーズの機能を起動する
            }
        }

        /// <summary>
        /// 視点移動の入力判定を行う関数
        /// </summary>
        /// <param name="context"></param>
        public void OnLook(InputAction.CallbackContext context)
        {
            inputHorizontal = context.ReadValue<float>();// キーが押されている間はその値（-1〜1）、離されたら0が入る
        }
    }
}