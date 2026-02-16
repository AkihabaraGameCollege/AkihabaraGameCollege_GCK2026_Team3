using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

/// <summary>
/// コード内で参照されているアセットパスを解析するユーティリティクラス。
/// </summary>
public static class CodeReferenceAnalyzer
{
    /// <summary>
    /// C#スクリプト内で参照されているアセットパスを抽出します。
    /// Resources.Load, Addressables.LoadAssetAsync, AssetDatabase.LoadAssetAtPath の呼び出しを検出します。
    /// </summary>
    /// <returns>参照されているアセットパスのセット</returns>
    public static HashSet<string> GetReferencedAssetPaths()
    {
        var result = new HashSet<string>();
        // プロジェクト内の全C#ファイルを取得
        var csFiles = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories);

        // アセット参照を検出するための正規表現リスト
        var regexList = new[]
        {
            // Resources.Load("パス") の検出
            new Regex(@"Resources\.Load\s*\(\s*\""(.*?)\""", RegexOptions.Compiled),
            // Addressables.LoadAssetAsync("パス") の検出
            new Regex(@"Addressables\.LoadAssetAsync\s*\(\s*\""(.*?)\""", RegexOptions.Compiled),
            // AssetDatabase.LoadAssetAtPath<T>("パス") の検出
            new Regex(@"AssetDatabase\.LoadAssetAtPath\s*<.*?>\s*\(\s*\""(.*?)\""", RegexOptions.Compiled)
        };

        // 各C#ファイルを解析
        foreach (var file in csFiles)
        {
            string text;
            try
            {
                // ファイル内容を読み込む
                text = File.ReadAllText(file);
            }
            catch (System.Exception ex)
            {
                // 読み込み失敗時は警告を表示してスキップ
                UnityEngine.Debug.LogWarning($"CodeReferenceAnalyzer: {file} の読み込み失敗: {ex.Message}");
                continue;
            }
            // 各正規表現でアセット参照を抽出
            foreach (var regex in regexList)
            {
                foreach (Match match in regex.Matches(text))
                {
                    var path = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(path))
                    {
                        // Resources.Load の場合はパスを補完
                        if (regex.ToString().Contains("Resources.Load"))
                            result.Add("Assets/Resources/" + path);
                        // Addressables, AssetDatabase の場合はそのまま追加
                        else if (regex.ToString().Contains("Addressables.LoadAssetAsync"))
                            result.Add(path);
                        else
                            result.Add(path);
                    }
                }
            }
        }
        return result;
    }
}
