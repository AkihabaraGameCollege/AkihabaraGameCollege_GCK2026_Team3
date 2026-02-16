using System.Collections.Generic;
using System.IO;

/// <summary>
/// アセットパスが除外対象かどうかを判定する静的クラス。
/// 除外フォルダ・拡張子のリストは CleanerSettings から取得します。
/// </summary>
public static class FilterManager
{
    /// <summary>
    /// 指定したアセットパスが除外対象かどうかを判定します。
    /// 除外フォルダまたは除外拡張子に一致する場合 true を返します。
    /// </summary>
    /// <param name="assetPath">判定するアセットのパス</param>
    /// <param name="settings">除外設定（フォルダ・拡張子リスト）</param>
    /// <returns>除外対象なら true、そうでなければ false</returns>
    public static bool IsExcluded(string assetPath, CleanerSettings settings)
    {
        // パス区切り文字を統一
        string normalizedPath = assetPath.Replace("\\", "/");

        // 除外フォルダに一致するか判定
        foreach (var folder in settings.excludedFolders)
        {
            string folderNorm = folder.Replace("\\", "/");
            // フォルダで始まる、または途中に含まれる場合は除外
            if (normalizedPath.StartsWith(folderNorm, System.StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.Contains("/" + folderNorm))
                return true;
        }

        // 除外拡張子に一致するか判定
        foreach (var ext in settings.excludedExtensions)
        {
            if (normalizedPath.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // いずれにも一致しない場合は除外対象外
        return false;
    }
}