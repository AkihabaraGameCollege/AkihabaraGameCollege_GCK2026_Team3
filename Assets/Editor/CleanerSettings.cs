using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 未使用アセットクリーナーの除外設定を管理する ScriptableObject。
/// 除外する拡張子やフォルダをリストで保持します。
/// </summary>
[CreateAssetMenu(fileName = "CleanerSettings", menuName = "UnusedAssetCleaner/CleanerSettings")]
public class CleanerSettings : ScriptableObject
{
    /// <summary>
    /// 未使用判定から除外するファイル拡張子のリスト。
    /// 例: スクリプト、シェーダー、メタファイルなど。
    /// </summary>
    public List<string> excludedExtensions = new List<string>
    {
        // スクリプト・設定
        ".cs", ".js", ".boo", ".asmdef", ".json", ".xml", ".dll",
        // Unity固有
        ".meta", ".unity", ".prefab", ".mat", ".controller", ".asset",
        // シェーダー・テクスチャ
        ".shader", ".cginc", ".compute", ".png", ".jpg", ".tga", ".psd", ".exr", ".hdr",
        // モデル・アニメーション
        ".fbx", ".obj", ".blend", ".anim", ".controller",
        // サウンド
        ".wav", ".mp3", ".ogg",
        // その他
        ".txt", ".csv"
    };

    /// <summary>
    /// 未使用判定から除外するフォルダのリスト。
    /// 例: Packages、Editor、Resources など。
    /// </summary>
    public List<string> excludedFolders = new List<string>
    {
        "Packages/",
        "Editor/",
        "Resources/",
        "StreamingAssets/",
        "Plugins/",
        "AddressableAssets/",
        "ThirdParty/",
        "Tests/",
        "Docs/",
        "Art/",
        "Audio/",
        "Models/"
    };
}