using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

/// <summary>
/// アセットをTrashフォルダへ移動・復旧する管理ユーティリティ。
/// 安全にアセットを一時的に隔離・復元できます。
/// </summary>
public static class TrashManager
{
    private const string TrashFolder = "Assets/Trash";

    /// <summary>
    /// 指定したアセットをTrashフォルダへ移動します。
    /// 既にTrashに同名ファイルがある場合は警告し、移動しません。
    /// </summary>
    /// <param name="assetPath">移動するアセットのパス</param>
    /// <returns>移動成功時 true、失敗時 false</returns>
    public static bool MoveToTrash(string assetPath)
    {
        // Trashフォルダが存在しない場合は作成
        if (!AssetDatabase.IsValidFolder(TrashFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Trash");
        }

        string fileName = Path.GetFileName(assetPath);
        string destPath = Path.Combine(TrashFolder, fileName);

        // Trash内に同名ファイルが存在する場合は警告
        if (File.Exists(destPath) || Directory.Exists(destPath))
        {
            EditorUtility.DisplayDialog(
                "Trashに同名ファイルあり",
                $"Trash内に同名ファイル {fileName} が既に存在します。移動できません。",
                "OK"
            );
            return false;
        }

        // ユーザーに移動確認ダイアログを表示
        bool confirm = EditorUtility.DisplayDialog(
            "アセットをTrashへ移動",
            $"{assetPath} を Trash に移動しますか？\n（復旧可能です）",
            "はい", "キャンセル"
        );
        if (!confirm) return false;

        // Undo履歴に記録
        Object assetObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (assetObj != null)
        {
            Undo.RecordObject(assetObj, "Move Asset To Trash");
        }
        try
        {
            // ファイル/ディレクトリをTrashへ移動
            FileUtil.MoveFileOrDirectory(assetPath, destPath);
            AssetDatabase.Refresh();

            // Unity再起動を必ず実行（選択肢なし）
            EditorUtility.DisplayDialog(
                "Unity再起動",
                "アセットをTrashに移動しました。Unityを再起動します。",
                "OK"
            );
            CreateAndRunRestartBatch();
            EditorApplication.Exit(0);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"TrashManager.MoveToTrash: {ex.Message}");
            EditorUtility.DisplayDialog("エラー", $"Trashへの移動に失敗しました: {ex.Message}", "OK");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 複数アセットをTrashフォルダへまとめて移動し、すべて成功したら再起動します。
    /// </summary>
    /// <param name="assetPaths">移動するアセットのパスリスト</param>
    /// <returns>移動成功数</returns>
    public static int MoveAllToTrash(List<string> assetPaths)
    {
        // Trashフォルダが存在しない場合は作成
        if (!AssetDatabase.IsValidFolder(TrashFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Trash");
        }

        int moveCount = 0;
        foreach (var assetPath in assetPaths)
        {
            string fileName = Path.GetFileName(assetPath);
            string destPath = Path.Combine(TrashFolder, fileName);

            // Trash内に同名ファイルが存在する場合は警告
            if (File.Exists(destPath) || Directory.Exists(destPath))
            {
                EditorUtility.DisplayDialog(
                    "Trashに同名ファイルあり",
                    $"Trash内に同名ファイル {fileName} が既に存在します。移動できません。",
                    "OK"
                );
                continue;
            }

            // Undo履歴に記録
            Object assetObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (assetObj != null)
            {
                Undo.RecordObject(assetObj, "Move Asset To Trash");
            }
            try
            {
                // ファイル/ディレクトリをTrashへ移動
                FileUtil.MoveFileOrDirectory(assetPath, destPath);
                moveCount++;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"TrashManager.MoveAllToTrash: {ex.Message}");
                EditorUtility.DisplayDialog("エラー", $"Trashへの移動に失敗しました: {ex.Message}", "OK");
            }
        }
        AssetDatabase.Refresh();

        // すべて移動できた場合は確認ダイアログなしですぐ再起動
        if (moveCount == assetPaths.Count && moveCount > 0)
        {
            CreateAndRunRestartBatch();
            EditorApplication.Exit(0);
        }
        // 一部のみ移動できた場合は従来通りダイアログ表示
        else if (moveCount > 0)
        {
            EditorUtility.DisplayDialog(
                "Unity再起動",
                $"{moveCount}件のアセットをTrashに移動しました。Unityを再起動します。",
                "OK"
            );
            CreateAndRunRestartBatch();
            EditorApplication.Exit(0);
        }
        return moveCount;
    }

    /// <summary>
    /// Unity Editor再起動用バッチファイルを生成して実行
    /// Editor終了後に新しいUnity Editorを起動することで、
    /// 複数インスタンスによる競合を防止します。
    /// </summary>
    private static void CreateAndRunRestartBatch()
    {
        string editorPath = EditorApplication.applicationPath;
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string batchPath = Path.Combine(Path.GetTempPath(), "RestartUnityEditor.bat");

        // バッチ内容: 2秒待機後にUnity Editorを起動
        string batchContent =
            "@echo off\r\n" +
            "timeout /t 2 /nobreak >nul\r\n" +
            $"start \"\" \"{editorPath}\" -projectPath \"{projectPath}\"\r\n";

        File.WriteAllText(batchPath, batchContent);

        ProcessStartInfo psi = new ProcessStartInfo(batchPath)
        {
            CreateNoWindow = true,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    /// <summary>
    /// Trashフォルダから指定ファイルを元のパスへ復旧します。
    /// 復旧先に同名ファイルがある場合は警告し、復旧しません。
    /// </summary>
    /// <param name="fileName">Trash内のファイル名</param>
    /// <param name="originalPath">復旧先の元パス</param>
    /// <returns>復旧成功時 true、失敗時 false</returns>
    public static bool RestoreFromTrash(string fileName, string originalPath)
    {
        string trashPath = Path.Combine(TrashFolder, fileName);

        // Trash内にファイルが存在しない場合は失敗
        if (!File.Exists(trashPath) && !Directory.Exists(trashPath))
        {
            UnityEngine.Debug.LogWarning($"TrashManager: {trashPath} が見つかりません。");
            return false;
        }

        // Undo履歴に記録（任意）
        Object assetObj = AssetDatabase.LoadMainAssetAtPath(trashPath);
        if (assetObj != null)
        {
            Undo.RecordObject(assetObj, "Restore Asset From Trash");
        }
        try
        {
            FileUtil.MoveFileOrDirectory(trashPath, originalPath);
            AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"TrashManager.RestoreFromTrash: {ex.Message}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Trashフォルダから指定ファイルを削除します。
    /// </summary>
    /// <param name="fileName">Trash内のファイル名</param>
    /// <returns>削除成功時 true、失敗時 false</returns>
    public static bool DeleteFromTrash(string fileName)
    {
        string trashPath = Path.Combine(TrashFolder, fileName);

        try
        {
            if (File.Exists(trashPath))
            {
                File.Delete(trashPath);
                // .metaファイルも削除
                string metaPath = trashPath + ".meta";
                if (File.Exists(metaPath))
                    File.Delete(metaPath);
            }
            else if (Directory.Exists(trashPath))
            {
                Directory.Delete(trashPath, true);
                // .metaファイルも削除
                string metaPath = trashPath + ".meta";
                if (File.Exists(metaPath))
                    File.Delete(metaPath);
            }
            AssetDatabase.Refresh();
            return true;
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"TrashManager.DeleteFromTrash: {ex.Message}");
            EditorUtility.DisplayDialog("エラー", $"Trashからの削除に失敗しました: {ex.Message}", "OK");
            return false;
        }
    }
}