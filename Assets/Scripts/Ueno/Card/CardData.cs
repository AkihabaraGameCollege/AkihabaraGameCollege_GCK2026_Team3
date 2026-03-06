//using unityengine;

//// scriptableobjectとしてカードデータを作成できるようにする属性
//// unityの「create > ueno > carddata」からアセット作成可能
//[createassetmenu(menuname = "ueno/carddata", filename = "carddata")]
//public class carddata : scriptableobject
//{
//    // カードの名前（ゲーム内表示用）
//    public string cardname;

//    // カードの種類（攻撃・防御など）
//    public cardtype cardtype;

//    // カードの効果タイプ（ダメージ系・回復系など）
//    public cardeffecttype effecttype;

//    // カードの説明文（inspector上で複数行入力できる）
//    [textarea]
//    public string description;

//    // カードを使用するためのコスト
//    public int cost;

//    // 最小ダメージ値（ランダム計算用）
//    public int mindamage;

//    // 最大ダメージ値（ランダム計算用）
//    public int maxdamage;

//    // カードのイラスト画像
//    public sprite artwork;
//}