using UnityEngine;
using UnityEngine.Audio;

public class Tornado : MonoBehaviour
{
    [Header("竜巻の吹き飛ばす力")]
    [SerializeField]
    private Vector3 tornadoForce = new Vector3(0, 10f, 0); // 竜巻の力を表すベクトル

    [Header("竜巻の効果音")]
    [SerializeField]
    private AudioSource tornadoAudioSource; // 竜巻の効果音を再生するためのAudioSource

    [Header("オーディオミキサー")]
    [SerializeField]
    private AudioMixer audioMixer; // オーディオミキサーの参照

    // tornadoForceのプロパティ
    public Vector3 TornadoForce => tornadoForce;

    private void Start()
    {
        // AudioSourceの設定を行う
        SetupAudioSource(tornadoAudioSource);
        // 効果音が再生されていない場合は再生を開始する
        if (!tornadoAudioSource.isPlaying)
        {
            tornadoAudioSource.Play();
        }
    }

    // AudioSourceの設定を行うメソッド
    private void SetupAudioSource(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            // "SEGroup"に一致するオーディオミキサーグループを取得
            var seGroups = audioMixer.FindMatchingGroups("SEGroup");
            if (seGroups.Length > 0)
            {
                // AudioSourceの出力先を設定
                audioSource.outputAudioMixerGroup = seGroups[0];
            }
        }
    }
}
