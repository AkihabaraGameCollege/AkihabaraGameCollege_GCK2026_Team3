using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// プロジェクト内の未使用アセットを検出するユーティリティクラス。
/// </summary>
public static class AssetScanner
{
    /// <summary>
    /// 進捗状況を通知するためのコールバックデリゲート。
    /// </summary>
    public delegate void ProgressCallback(float progress, string message);
    /// <summary>
    /// ステータスを通知するためのコールバックデリゲート。
    /// </summary>
    public delegate void StatusCallback(string status, string message);

    private const string CleanerSettingsPath = "Assets/Editor/CleanerSettings.asset";
    private const string TrashFolder = "Assets/Trash";

    /// <summary>
    /// 未使用アセットをスキャンし、パスのリストとして返します。
    /// </summary>
    /// <param name="onProgress">進捗通知用コールバック</param>
    /// <param name="onStatus">ステータス通知用コールバック</param>
    /// <returns>未使用アセットのパス一覧</returns>
    public static List<string> ScanUnusedAssets(ProgressCallback onProgress = null, StatusCallback onStatus = null)
    {
        // 設定ファイルのロード
        var settings = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
        // プロジェクト内の全アセットパスを取得
        string[] allAssets = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets/"))
            .ToArray();

        HashSet<string> usedAssets = new HashSet<string>();
        int totalSteps = allAssets.Length + EditorBuildSettings.scenes.Length + 2;
        int currentStep = 0;

        // シーン依存アセットを収集
        foreach (var scene in EditorBuildSettings.scenes)
        {
            string[] deps = AssetDatabase.GetDependencies(scene.path, true);
            foreach (var d in deps)
                usedAssets.Add(d);
            currentStep++;
            onProgress?.Invoke((float)currentStep / totalSteps, $"シーン依存解析: {scene.path}");
        }

        // Addressableアセットを収集
        foreach (var addr in AddressableIntegration.GetAddressableAssetPaths())
            usedAssets.Add(addr);
        currentStep++;
        onProgress?.Invoke((float)currentStep / totalSteps, "Addressables依存解析");

        // コード参照アセットを収集
        foreach (var codeRef in CodeReferenceAnalyzer.GetReferencedAssetPaths())
            usedAssets.Add(codeRef);
        currentStep++;
        onProgress?.Invoke((float)currentStep / totalSteps, "コード参照解析");

        var unused = new List<string>();
        // 未使用アセットを判定（除外リスト判定を外す）
        for (int i = 0; i < allAssets.Length; i++)
        {
            var a = allAssets[i];
            // Trashフォルダは除外
            if ((a.StartsWith(TrashFolder + "/") || a.Equals(TrashFolder)))
                continue;

            // 使用されていない場合、未使用リストに追加（除外リストは考慮しない）
            if (!usedAssets.Contains(a))
                unused.Add(a);

            // 100件ごとに進捗通知
            if (i % 100 == 0)
                onProgress?.Invoke((float)(currentStep + i) / totalSteps, $"未使用アセット判定: {a}");
        }

        // スキャン完了通知
        onStatus?.Invoke("完了", $"未使用アセット数: {unused.Count}");
        return unused;
    }

    /// <summary>
    /// コマンドライン用の未使用アセットスキャン。結果をテキストファイルに出力します。
    /// </summary>
    public static void ScanUnusedAssetsCLI()
    {
        var unused = ScanUnusedAssets();
        File.WriteAllLines("UnusedAssetsReport.txt", unused);
        Debug.Log($"Unused asset scan complete. {unused.Count} assets found. Output: UnusedAssetsReport.txt");
    }

    /// <summary>
    /// 未使用アセットの差分を取得します。
    /// </summary>
    /// <param name="previousReportPath">前回のレポートファイルパス</param>
    /// <returns>追加・削除された未使用アセットのリスト</returns>
    public static (List<string> added, List<string> removed) DiffUnusedAssets(string previousReportPath)
    {
        var current = ScanUnusedAssets();
        var previous = File.Exists(previousReportPath)
            ? File.ReadAllLines(previousReportPath).ToList()
            : new List<string>();

        var added = current.Except(previous).ToList();
        var removed = previous.Except(current).ToList();
        return (added, removed);
    }

    /// <summary>
    /// 未使用アセットをパスとInstanceIDのペアで取得します。
    /// </summary>
    /// <param name="onProgress">進捗通知用コールバック</param>
    /// <param name="onStatus">ステータス通知用コールバック</param>
    /// <returns>未使用アセットのパスとInstanceIDのリスト</returns>
    public static List<(string path, int instanceId)> ScanUnusedAssetsWithInstanceID(
        ProgressCallback onProgress = null, StatusCallback onStatus = null)
    {
        // 設定ファイルのロード
        var settings = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
        // プロジェクト内の全アセットパスを取得
        string[] allAssets = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets/"))
            .ToArray();

        HashSet<string> usedAssets = new HashSet<string>();
        int totalSteps = allAssets.Length + EditorBuildSettings.scenes.Length + 2;
        int currentStep = 0;

        // シーン依存アセットを収集
        foreach (var scene in EditorBuildSettings.scenes)
        {
            string[] deps = AssetDatabase.GetDependencies(scene.path, true);
            foreach (var d in deps)
                usedAssets.Add(d);
            currentStep++;
            onProgress?.Invoke((float)currentStep / totalSteps, $"シーン依存解析: {scene.path}");
        }

        // Addressableアセットを収集
        foreach (var addr in AddressableIntegration.GetAddressableAssetPaths())
            usedAssets.Add(addr);
        currentStep++;
        onProgress?.Invoke((float)currentStep / totalSteps, "Addressables依存解析");

        // コード参照アセットを収集
        foreach (var codeRef in CodeReferenceAnalyzer.GetReferencedAssetPaths())
            usedAssets.Add(codeRef);
        currentStep++;
        onProgress?.Invoke((float)currentStep / totalSteps, "コード参照解析");

        var unused = new List<(string path, int instanceId)>();
        // 未使用アセットを判定
        for (int i = 0; i < allAssets.Length; i++)
        {
            var a = allAssets[i];
            // Trashフォルダは除外
            if ((a.StartsWith(TrashFolder + "/") || a.Equals(TrashFolder)))
                continue;

            // 使用されていないかつ除外設定に該当しない場合、未使用リストに追加
            if (!usedAssets.Contains(a) && (settings == null || !FilterManager.IsExcluded(a, settings)))
            {
                UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(a);
                int instanceId = obj != null ? obj.GetInstanceID() : -1;
                unused.Add((a, instanceId));
            }

            // 100件ごとに進捗通知
            if (i % 100 == 0)
                onProgress?.Invoke((float)(currentStep + i) / totalSteps, $"未使用アセット判定: {a}");
        }

        // スキャン完了通知
        onStatus?.Invoke("完了", $"未使用アセット数: {unused.Count}");
        return unused;
    }
}