// 必要な名前空間をインポート
using System.Collections.Generic;
using UnityEngine;

public class ParticleResetter : MonoBehaviour
{
    // インスペクターで設定可能なパーティクルシステムのリスト
    [Header("パーティクルシステムのリスト")]
    [SerializeField]
    private List<ParticleSystem> particleSystems;

    // インスペクターで設定可能なリセット間隔（秒）
    [Header("リセット間隔（秒）")]
    [SerializeField]
    private float resetInterval = 2f;

    // リセットタイマー
    private float resetTimer = 0f;

    // 毎フレーム呼び出されるUpdateメソッド
    void Update()
    {
        // 経過時間を加算
        resetTimer += Time.deltaTime;

        // リセット間隔に達したらパーティクルシステムをリセット
        if (resetTimer >= resetInterval)
        {
            ResetParticlePlaybackTime();
            // タイマーをリセット
            resetTimer = 0f;
        }
    }

    // パーティクルシステムの再生時間をリセットするメソッド
    private void ResetParticlePlaybackTime()
    {
        // 各パーティクルシステムに対して処理を行う
        foreach (var particleSystem in particleSystems)
        {
            var main = particleSystem.main;
            // スタートディレイを0に設定
            main.startDelay = 0f;
            // パーティクルシステムの時間をリセット
            particleSystem.time = 0f;
            // パーティクルシステムを再生
            particleSystem.Play();
        }
    }
}
