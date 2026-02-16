using UnityEditor;
using System.Collections.Generic;
#if UNITY_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

/// <summary>
/// Addressableアセットのパスを取得するためのユーティリティクラス。
/// </summary>
public static class AddressableIntegration
{
    /// <summary>
    /// Addressableに登録されている全アセットのパスを取得します。
    /// サブアセット（Spriteなど）も含まれます。
    /// </summary>
    /// <returns>Addressableに登録されているアセットのパス一覧</returns>
    public static HashSet<string> GetAddressableAssetPaths()
    {
        var result = new HashSet<string>();
#if UNITY_ADDRESSABLES
        // Addressableの設定オブジェクトを取得
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null)
        {
            // 各グループを走査
            foreach (var group in settings.groups)
            {
                // グループ内の各エントリ（アセット）を走査
                foreach (var entry in group.entries)
                {
                    if (!string.IsNullOrEmpty(entry.AssetPath))
                    {
                        // アセットのパスを追加
                        result.Add(entry.AssetPath);

                        // サブアセット（Spriteなど）のパスも取得して追加
                        var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(entry.AssetPath);
                        foreach (var sub in subAssets)
                        {
                            var subPath = AssetDatabase.GetAssetPath(sub);
                            if (!string.IsNullOrEmpty(subPath))
                                result.Add(subPath);
                        }
                    }
                }
            }
        }
#endif
        return result;
    }
}
