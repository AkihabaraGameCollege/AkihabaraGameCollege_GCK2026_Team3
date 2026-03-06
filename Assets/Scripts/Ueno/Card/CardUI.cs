//using UnityEngine;
//using UnityEngine.EventSystems;

//// カードの見た目（UI）と操作（クリック・ドラッグ）を管理するクラス
//public class CardUI : MonoBehaviour
//{
//    // このUIが持っているカードデータ
//    public CardData cardData;

//    // ドラッグ中かどうか
//    bool dragging = false;

//    // 元の位置（ドラッグ後に戻すため）
//    Vector3 startPos;

//    // 追加: 選択表示用フラグ
//    bool selected = false;

//    // 元のスケール（拡大表示から戻すため）
//    Vector3 originalScale;

//    void Start()
//    {
//        // 初期位置と初期スケールを保存
//        startPos = transform.position;
//        originalScale = transform.localScale;
//    }

//    void Update()
//    {
//        // ドラッグ中の処理
//        if (dragging)
//        {
//            // マウス位置を取得
//            Vector3 mouse = Input.mousePosition;

//            // カメラからの距離（ScreenToWorldPoint用）
//            mouse.z = 10f;

//            // マウス位置をワールド座標に変換してカードを移動
//            transform.position = Camera.main.ScreenToWorldPoint(mouse);

//            // 左クリックを離したら
//            if (Input.GetMouseButtonUp(0))
//            {
//                dragging = false;

//                // カードを使用できるか試す
//                TryPlay();

//                // 位置を元に戻す
//                transform.position = startPos;
//            }
//        }
//    }

//    // マウスでカードを押したとき（レガシーInput）
//    void OnMouseDown()
//    {
//        dragging = true;
//    }

//    // TextMeshProのボタンから呼び出す用
//    // 押すたびに「選択状態」を切り替える
//    public void OnTMPButtonToggleSelect()
//    {
//        if (!selected) Select();
//        else Deselect();
//    }

//    // ボタンから直接カードを使用する場合に呼ぶ
//    public void PlayFromButton()
//    {
//        // 選択状態を解除
//        Deselect();

//        // カード使用処理
//        TryPlay();

//        // 位置を元に戻す
//        transform.position = startPos;
//    }

//    // カードを選択状態にする（拡大表示）
//    public void Select()
//    {
//        selected = true;

//        // 少し拡大して視覚的に強調
//        transform.localScale = originalScale * 1.08f;

//        // TODO:
//        // 色変更や枠の表示などを追加すると
//        // より分かりやすい選択表現が可能
//    }

//    // 選択解除（元のサイズに戻す）
//    public void Deselect()
//    {
//        selected = false;
//        transform.localScale = originalScale;
//    }

//    // カードを実際に使用する処理
//    void TryPlay()
//    {
//        if (cardData == null) return;

//        // CardManagerに使用可能か問い合わせる
//        bool ok = CardManager.Instance.PlayCard(cardData);

//        if (!ok)
//        {
//            // 使用できなかった場合（コスト不足など）
//            Debug.Log("Cannot play card: " + cardData.cardName);
//        }
//        else
//        {
//            // 使用成功時は選択状態を解除
//            Deselect();
//        }
//    }
//}