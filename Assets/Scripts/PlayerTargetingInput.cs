using UnityEngine;
using UnityEngine.InputSystem;

// プレイヤー入力でターゲット操作を行う補助スクリプト。
// - クリックアクション (Button) : マウス直下の Enemy をターゲット
// - カーソル位置アクション (Vector2) : クリック時のスクリーン座標（未設定なら Mouse.current を使用）
// - cycleAction (Button) : 最寄りの Enemy をターゲット（見つからなければ解除）
// - cancelAction (Button) : ターゲット解除
public class PlayerTargetingInput : MonoBehaviour
{
    Camera mainCam;

    [SerializeField, Tooltip("Tabで検索する最大距離")] float searchRadius = 30f;
    [SerializeField, Tooltip("クリック判定の最大距離")] float clickRayDistance = 100f;

    // Input System 用アクション参照（Inspectorで Input Actions のアクションを割り当てる）
    [SerializeField, Tooltip("クリックボタンの InputAction (Button)")]
    InputActionReference clickAction;
    [SerializeField, Tooltip("カーソル位置の InputAction (Vector2)。未割当時は Mouse.current.position を使用")]
    InputActionReference aimPositionAction;
    [SerializeField, Tooltip("ターゲット切替の InputAction (Button)")]
    InputActionReference cycleAction;
    [SerializeField, Tooltip("キャンセル（ターゲット解除）の InputAction (Button)")]
    InputActionReference cancelAction;

    void Start()
    {
        mainCam = Camera.main;
    }

    void OnEnable()
    {
        if (clickAction != null && clickAction.action != null)
        {
            clickAction.action.performed += OnClick;
            if (!clickAction.action.enabled) clickAction.action.Enable();
        }
        if (aimPositionAction != null && aimPositionAction.action != null)
        {
            if (!aimPositionAction.action.enabled) aimPositionAction.action.Enable();
        }
        if (cycleAction != null && cycleAction.action != null)
        {
            cycleAction.action.performed += OnCycle;
            if (!cycleAction.action.enabled) cycleAction.action.Enable();
        }
        if (cancelAction != null && cancelAction.action != null)
        {
            cancelAction.action.performed += OnCancel;
            if (!cancelAction.action.enabled) cancelAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (clickAction != null && clickAction.action != null)
        {
            clickAction.action.performed -= OnClick;
            if (clickAction.action.enabled) clickAction.action.Disable();
        }
        if (aimPositionAction != null && aimPositionAction.action != null)
        {
            if (aimPositionAction.action.enabled) aimPositionAction.action.Disable();
        }
        if (cycleAction != null && cycleAction.action != null)
        {
            cycleAction.action.performed -= OnCycle;
            if (cycleAction.action.enabled) cycleAction.action.Disable();
        }
        if (cancelAction != null && cancelAction.action != null)
        {
            cancelAction.action.performed -= OnCancel;
            if (cancelAction.action.enabled) cancelAction.action.Disable();
        }
    }

    // クリック処理（Input System のコールバック）
    void OnClick(InputAction.CallbackContext ctx)
    {

        Vector2 screenPos;
        if (aimPositionAction != null && aimPositionAction.action != null)
        {
            screenPos = aimPositionAction.action.ReadValue<Vector2>();
        }
        else if (Mouse.current != null)
        {
            screenPos = Mouse.current.position.ReadValue();
        }
        else
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, clickRayDistance))
        {
            var hitTransform = hit.collider.transform;
            if (hitTransform == null) return;
            if (hitTransform.CompareTag("Enemy") || hitTransform.GetComponent<StatusManager>() != null)
            {
            }
        }
    }

    // ターゲット切替（最寄り選択）
    void OnCycle(InputAction.CallbackContext ctx)
    {
    }

    // キャンセル（ターゲット解除）
    void OnCancel(InputAction.CallbackContext ctx)
    {
    }
}