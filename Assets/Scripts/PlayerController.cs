using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro; // TextMeshProの名前空間を追加

public class PlayerController : MonoBehaviour
{
    [Header("通常の移動速度")]
    [SerializeField]
    private float moveSpeed = 5f;  // 通常の移動速度

    [Header("スプリントの移動速度")]
    [SerializeField]
    private float sprintSpeed = 10f;  // スプリント時の移動速度

    [Header("ジャンプ力の設定")]
    [SerializeField]
    private Vector3 jumpForce = new(0, 5f, 0);  // ジャンプ力

    [Header("ジャンプ最大回数設定")]
    [SerializeField]
    private int maxJumpCount = 1;  // ジャンプの最大回数

    [Header("HP(耐久値)の設定")]
    [SerializeField]
    public float HPFillAmount = 180f;  // HPの初期値

    [Header("歩行時のHP減少率")]
    [SerializeField]
    private float walkHPDecreaseRate = 1f;  // 歩行時のHP減少率

    [Header("スプリント時のHP減少率")]
    [SerializeField]
    private float sprintHPDecreaseRate = 1.25f;  // スプリント時のHP減少率

    private Vector2 moveInput = Vector2.zero;  // プレイヤーの入力した移動方向
    private bool isSprinting = false;  // スプリント中かどうか
    private new Rigidbody rigidbody;  // プレイヤーのRigidbodyコンポーネント

    [Header("シネマシーンカメラの設定")]
    [SerializeField]
    private CinemachineVirtualCameraBase cinemachineCamera;  // シネマシーンカメラ

    [Header("停止時のパーティクル")]
    [SerializeField]
    private ParticleSystem idleParticleSystem;  // 停止時のパーティクル

    [Header("歩行時のパーティクル")]
    [SerializeField]
    private ParticleSystem walkParticleSystem;  // 歩行時のパーティクル

    [Header("スプリント時のパーティクル")]
    [SerializeField]
    private ParticleSystem sprintParticleSystem;  // スプリント時のパーティクル

    private int currentJumpCount = 0;  // 現在のジャンプ回数

    [Header("HP UIのImageコンポーネント")]
    [SerializeField]
    public Image hpUIImage;  // HPを表示するUIのImage

    private bool isReversed = false;  // 操作が反転しているかどうか

    [Header("操作反転の持続時間")]
    [SerializeField]
    private float reverseDuration = 5f;  // 操作反転の持続時間

    private float reverseTimer = 0f;  // 反転操作のタイマー

    [Header("サウンドマネージャー")]
    [SerializeField]
    private SoundManager soundManager;  // サウンドマネージャー

    [Header("ゲームオーバータイムのテキストメッシュプロ")]
    [SerializeField]
    private TextMeshProUGUI gameOverTimeText; // ゲームオーバータイムを表示するTextMeshProUGUI

    // ポーズマネージャーの参照
    private PauseManager _pauseManager;

    private bool particleSettingsChanged = false;  // パーティクル設定が変更されたかどうか
    private bool isGrounded = false;  // 地面に接触しているかどうか
    public bool isGameOver = false;  // ゲームオーバー状態を示すフラグ

    private void Awake()
    {
        // PauseManagerの参照を取得
        _pauseManager = FindFirstObjectByType<PauseManager>();
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();  // Rigidbodyコンポーネントを取得

        // シネマシーンカメラの初期設定
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = transform;
            cinemachineCamera.LookAt = transform;
        }
    }

    void Update()
    {
        HandleSprinting();  // スプリントの処理
        HandleSound();  // サウンドの処理
        if (particleSettingsChanged)
        {
            UpdateParticleSystem();  // パーティクルシステムの更新
            particleSettingsChanged = false;
        }
        UpdateHP();  // HPの更新
        UpdateReverseTimer();  // 反転操作タイマーの更新
    }

    private void FixedUpdate()
    {
        Move();  // 移動処理
        HandleWalkSound();  // 歩行音の処理
    }

    private void Move()
    {
        if (Camera.main != null)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            cameraForward.y = 0;  // カメラの上下を無視
            cameraRight.y = 0;

            // 入力に基づいて移動方向を決定
            Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

            // 操作が反転している場合、移動方向も反転
            if (isReversed)
            {
                moveDirection = -moveDirection;
            }

            // スプリント中か歩行中かで速度を切り替え
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            Vector3 force = moveDirection * currentSpeed;

            // Rigidbodyに力を加えて移動
            rigidbody.AddForce(force, ForceMode.Force);
        }
    }

    private void Jump()
    {
        if (currentJumpCount < maxJumpCount)
        {
            rigidbody.AddForce(jumpForce, ForceMode.Impulse);  // ジャンプ力を加える
            currentJumpCount++;  // ジャンプ回数を増加
            Debug.Log("jump");
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();  // プレイヤーの移動入力を取得
        particleSettingsChanged = true;  // パーティクル設定が変更されたことを記録
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
            soundManager.PlaySprintAudio();  // スプリント音を再生
            soundManager.StopWalkAudio();  // 歩行音を停止
            particleSettingsChanged = true;  // パーティクル設定が変更されたことを記録
        }
        else if (context.canceled)
        {
            isSprinting = false;
            soundManager.StopSprintAudio();  // スプリント音を停止
            soundManager.PlayWalkAudio();  // 歩行音を再生
            particleSettingsChanged = true;  // パーティクル設定が変更されたことを記録
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();  // ジャンプ処理
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        _pauseManager.TogglePause();  // ポーズの切り替え
    }

    public void StopAllMove()//プレイヤーの全体動きを止める
    {
        rigidbody.Sleep();
    }

    private void UpdateParticleSystem()
    {
        // すべてのパーティクルシステムを停止
        //idleParticleSystem.Stop();
        walkParticleSystem.Stop();
        sprintParticleSystem.Stop();

        // すべてのパーティクルシステムを非アクティブに設定
        idleParticleSystem.gameObject.SetActive(false);
        walkParticleSystem.gameObject.SetActive(false);
        sprintParticleSystem.gameObject.SetActive(false);

        // スプリントか歩行か停止かでパーティクルシステムを切り替え
        if (moveInput == Vector2.zero)
        {
            //idleParticleSystem.gameObject.SetActive(true);
            //idleParticleSystem.Play();
        }
        else if (isSprinting)
        {
            sprintParticleSystem.gameObject.SetActive(true);
            sprintParticleSystem.Play();
        }
        else
        {
            walkParticleSystem.gameObject.SetActive(true);
            walkParticleSystem.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentJumpCount = 0;  // 地面に着地したらジャンプ回数をリセット
            isGrounded = true;  // 地面に接触していることを記録
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // 地面に接触していることを記録
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;  // 地面から離れたことを記録
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 竜巻に触れているときに吹き飛ばす
        if (other.gameObject.CompareTag("Tornado"))
        {
            Tornado tornado = other.GetComponent<Tornado>();
            if (tornado != null)
            {
                rigidbody.AddForce(tornado.TornadoForce, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ReverseControl"))
        {
            isReversed = true;  // 操作反転を有効にする
            reverseTimer = reverseDuration;  // 操作反転のタイマー開始
            soundManager.PlayReverseControlAudio();  // 操作反転の効果音を再生
        }
        else if (other.CompareTag("Checkpoint"))
        {
            RestoreHP();  // 耐久値を全回復
        }
    }

    public void RestoreHP()
    {
        HPFillAmount = 180f;  // 耐久値を全回復
        if (hpUIImage != null)
        {
            hpUIImage.fillAmount = HPFillAmount / 180f;  // HP UIの更新
        }
    }

    private void UpdateHP()
    {
        // 移動中にHPを減少
        if (moveInput != Vector2.zero)
        {
            if (isSprinting)
            {
                HPFillAmount -= sprintHPDecreaseRate * Time.deltaTime;  // スプリント時のHP減少
            }
            else
            {
                HPFillAmount -= walkHPDecreaseRate * Time.deltaTime;  // 歩行時のHP減少
            }

            // HPが0以下にならないように制限
            HPFillAmount = Mathf.Max(HPFillAmount, 0);

            // HP UIを更新
            if (hpUIImage != null)
            {
                hpUIImage.fillAmount = HPFillAmount / 180f;  // HP UIの更新
            }

            // HPが0になった場合、ゲームオーバー処理を実行
            if (HPFillAmount <= 0)
            {
                HandleGameOver();
            }
        }
    }

    private void HandleSprinting()
    {
        bool wasSprinting = isSprinting;
        isSprinting = ((Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) ||
                       (Gamepad.current != null && Gamepad.current.rightShoulder.isPressed)) &&
                       moveInput != Vector2.zero;

        // スプリント状態の音の切り替え
        if (isSprinting && !wasSprinting)
        {
            soundManager.PlaySprintAudio();  // スプリント音を再生
            soundManager.StopWalkAudio();  // 歩行音を停止
            particleSettingsChanged = true;  // パーティクル設定が変更されたことを記録
        }
        else if (!isSprinting && wasSprinting)
        {
            soundManager.StopSprintAudio();  // スプリント音を停止
            soundManager.PlayWalkAudio();  // 歩行音を再生
            particleSettingsChanged = true;  // パーティクル設定が変更されたことを記録
        }
    }

    private void HandleSound()
    {
        // 歩行音の再生/停止
        if (moveInput != Vector2.zero && !isSprinting)
        {
            soundManager.PlayWalkAudio();  // 歩行音を再生
        }
        else if (moveInput == Vector2.zero)
        {
            soundManager.StopWalkAudio();  // 歩行音を停止
        }
    }

    private void HandleWalkSound()
    {
        if (isGrounded && moveInput != Vector2.zero && !isSprinting)
        {
            soundManager.PlayWalkAudio();  // 歩行音を再生
        }
        else
        {
            soundManager.StopWalkAudio();  // 歩行音を停止
        }
    }

    private void UpdateReverseTimer()
    {
        // 操作反転タイマーの更新
        if (isReversed)
        {
            reverseTimer -= Time.deltaTime;
            if (reverseTimer <= 0)
            {
                isReversed = false;  // 操作反転を解除
            }
        }
    }

    /// プレイヤーの状態をリセットするメソッド。
    /// 移動入力、スプリント状態、ジャンプ回数、操作反転状態、反転タイマー、HPを初期状態に戻します。
    public void ResetState()
    {
        moveInput = Vector2.zero;   // 移動入力をリセット

        isSprinting = false;// スプリント状態をリセット

        currentJumpCount = 0;   // ジャンプ回数をリセット

        isReversed = false; // 操作反転状態をリセット

        reverseTimer = 0f;  // 反転操作のタイマーをリセット

        HPFillAmount = 180f;    // HPを初期値にリセット

        if (hpUIImage != null)  // HP UIを更新
        {
            hpUIImage.fillAmount = HPFillAmount / 180f;
        }
    }

    public void HandleGameOver()
    {
        // GameDirectoryの参照を取得
        GameDirectory gameDirectory = FindFirstObjectByType<GameDirectory>();
        if (gameDirectory != null)
        {
            gameDirectory.SetGameOver();
            // ゲームオーバータイムを表示
            if (gameOverTimeText != null && gameDirectory.timerText != null)
            {
                gameOverTimeText.text = gameDirectory.timerText.text; // timerTextの値を参照して表示
            }
        }

        // ゲームオーバーUIを表示
        UI_Anim uiAnim = FindFirstObjectByType<UI_Anim>();
        if (uiAnim != null)
        {
            uiAnim.OpenGameoverUI();
        }

        // サウンドマネージャーの参照を取得
        SoundManager soundManager = FindFirstObjectByType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.PlayGameOverAudio();
        }

        // プレイヤー操作を無効にする
        enabled = false;
    }
}
