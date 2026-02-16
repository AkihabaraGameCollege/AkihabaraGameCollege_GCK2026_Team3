using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UnusedAssetCleaner用の表示フィルター設定を管理するScriptableObject。
/// 指定したフォルダや拡張子を含めることで、
/// アセットクリーナーの表示対象を制御する。
/// </summary>
[CreateAssetMenu(fileName = "DisplayFilterSettings", menuName = "UnusedAssetCleaner/DisplayFilterSettings")]
public class DisplayFilterSettings : ScriptableObject
{
    /// <summary>
    /// 表示対象に含めるフォルダのリスト（実体）。
    /// </summary>
    public List<string> includeFolders = new List<string>
    {
        "Assets/Editor",
        "Assets/Plugins",
        "Assets/StreamingAssets",
        "Assets/AddressableAssetsData",
        "Assets/Gizmos",
        "Assets/Resources",
        "Assets/Settings",
        "Assets/Documentation",
        "Assets/Tests",
        "Assets/Readme"
    };

    /// <summary>
    /// 表示対象に含める拡張子のリスト（実体）。
    /// </summary>
    public List<string> includeExtensions = new List<string>
    {
        ".meta", ".md", ".txt", ".pdf", ".csv", ".json", ".xml", ".dll", ".asmdef", ".unitypackage", ".cs", ".shader", ".cginc"
    };

    /// <summary>
    /// フォルダのローカライズ辞書
    /// キー: 実体パス, 値: (ja, en, zh, zht)
    /// </summary>
    public Dictionary<string, (string ja, string en, string zh, string zht)> folderLabels =
        new Dictionary<string, (string, string, string, string)>
    {
        { "Assets/Editor", ("エディタ", "Editor", "编辑器", "編輯器") },
        { "Assets/Plugins", ("プラグイン", "Plugins", "插件", "插件") },
        { "Assets/StreamingAssets", ("ストリーミングアセット", "StreamingAssets", "流式资源", "串流資源") },
        { "Assets/AddressableAssetsData", ("アドレッサブル", "Addressables", "可寻址", "可尋址") },
        { "Assets/Gizmos", ("ギズモ", "Gizmos", "Gizmos", "Gizmos") },
        { "Assets/Resources", ("リソース", "Resources", "资源", "資源") },
        { "Assets/Settings", ("設定", "Settings", "设置", "設定") },
        { "Assets/Documentation", ("ドキュメント", "Documentation", "文档", "文件") },
        { "Assets/Tests", ("テスト", "Tests", "测试", "測試") },
        { "Assets/Readme", ("Readme", "Readme", "Readme", "Readme") }
    };

    /// <summary>
    /// 拡張子のローカライズ辞書
    /// キー: 拡張子, 値: (ja, en, zh, zht)
    /// </summary>
    public Dictionary<string, (string ja, string en, string zh, string zht)> extensionLabels =
        new Dictionary<string, (string, string, string, string)>
    {
        { ".meta", ("メタ", "Meta", "Meta", "Meta") },
        { ".md", ("Markdown", "Markdown", "Markdown", "Markdown") },
        { ".txt", ("テキスト", "Text", "文本", "純文字") },
        { ".pdf", ("PDF", "PDF", "PDF", "PDF") },
        { ".csv", ("CSV", "CSV", "CSV", "CSV") },
        { ".json", ("JSON", "JSON", "JSON", "JSON") },
        { ".xml", ("XML", "XML", "XML", "XML") },
        { ".dll", ("DLL", "DLL", "DLL", "DLL") },
        { ".asmdef", ("アセンブリ定義", "Assembly Definition", "程序集定义", "組件定義") },
        { ".unitypackage", ("Unityパッケージ", "Unity Package", "Unity包", "Unity套件") },
        { ".cs", ("C#スクリプト", "C# Script", "C#脚本", "C#腳本") },
        { ".shader", ("シェーダー", "Shader", "着色器", "著色器") },
        { ".cginc", ("CGインクルード", "CG Include", "CG包含", "CG包含") }
    };
}
