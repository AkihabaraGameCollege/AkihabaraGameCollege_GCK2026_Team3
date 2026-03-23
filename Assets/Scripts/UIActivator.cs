using UnityEngine;
using UnityEngine.UI;

public class UIActivator : MonoBehaviour
{
    [Header("アクティブにするUIのImageコンポーネント")]
    [SerializeField]
    private Image targetImage;  // アクティブにするUIのImageコンポーネント

    // プレイヤーがトリガーに入ったときに呼び出される
    private void OnTriggerEnter(Collider other)
    {
        // トリガーに入ったオブジェクトがプレイヤーかどうかを確認
        if (other.CompareTag("Player"))
        {
            // プレイヤーが触れたらUIをアクティブにする
            targetImage.gameObject.SetActive(true);
        }
    }

    // プレイヤーがトリガーから出たときに呼び出される
    private void OnTriggerExit(Collider other)
    {
        // トリガーから出たオブジェクトがプレイヤーかどうかを確認
        if (other.CompareTag("Player"))
        {
            // プレイヤーが離れたらUIを非アクティブにする
            targetImage.gameObject.SetActive(false);
        }
    }
}
