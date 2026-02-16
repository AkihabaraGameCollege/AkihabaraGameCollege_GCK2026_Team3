using System.Collections.Generic;
using System.IO;

/// <summary>
/// DisplayFilterSettingsの条件に基づき、アセットパスが表示対象かどうかを判定するマネージャクラス。
/// </summary>
public static class DisplayFilterManager
{
    /// <summary>
    /// 指定したアセットパスがDisplayFilterSettingsの条件に含まれるか判定する。
    /// </summary>
    /// <param name="assetPath">判定対象のアセットパス</param>
    /// <param name="settings">フィルター条件を保持するDisplayFilterSettingsインスタンス</param>
    /// <returns>条件に含まれる場合はtrue、含まれない場合はfalse</returns>
    public static bool IsIncluded(string assetPath, DisplayFilterSettings settings)
    {
        // パスの区切り文字を統一（バックスラッシュ→スラッシュ）
        string normalizedPath = assetPath.Replace("\\", "/");

        // includeFoldersに含まれるフォルダパスで始まる、または途中に含まれていればtrue
        foreach (var folder in settings.includeFolders)
        {
            string folderNorm = folder.Replace("\\", "/");
            if (normalizedPath.StartsWith(folderNorm, System.StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.Contains("/" + folderNorm))
                return true;
        }

        // includeExtensionsに含まれる拡張子で終わっていればtrue
        foreach (var ext in settings.includeExtensions)
        {
            if (normalizedPath.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // いずれにも該当しない場合はfalse
        return false;
    }
}
