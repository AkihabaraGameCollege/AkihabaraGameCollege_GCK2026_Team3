using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// CleanerSettingsの除外リスト（フォルダ・拡張子）を編集するためのエディタウィンドウ。
/// ローカライズ対応。
/// </summary>
public class ExcludedListEditorWindow : EditorWindow
{
    // 除外設定データ
    private CleanerSettings settings;
    // スクロール位置
    private Vector2 scroll;

    // UnusedAssetCleanerWindow からローカライズ辞書と言語を取得
    private static System.Func<string, string> T =>
        UnusedAssetCleanerWindow.GetTranslation;

    private const string CleanerSettingsPath = "Assets/Editor/CleanerSettings.asset";

    // 入力バリデーション用
    private static bool IsValidFolder(string folder)
    {
        // 空文字・空白不可、スラッシュで始まる/終わる以外はOK
        return !string.IsNullOrWhiteSpace(folder) &&
               !folder.Contains("\\") &&
               !folder.Contains("//") &&
               !folder.Contains("..") &&
               Regex.IsMatch(folder, @"^[\w\-/]+/?$");
    }
    private static bool IsValidExtension(string ext)
    {
        // 空文字不可、.で始まり英数字のみ
        return !string.IsNullOrWhiteSpace(ext) &&
               ext.StartsWith(".") &&
               Regex.IsMatch(ext, @"^\.[a-zA-Z0-9]+$");
    }

    /// <summary>
    /// メニューからウィンドウを表示する（タイトルもローカライズ対応）
    /// </summary>
    [MenuItem("Tools/除外リスト編集")]
    public static void ShowWindow()
    {
        // タイトル・タブ名をローカライズ
        GetWindow<ExcludedListEditorWindow>(T("exclusionListEditorWindow"));
    }

    /// <summary>
    /// ウィンドウ有効化時にCleanerSettingsをロード
    /// </summary>
    private void OnEnable()
    {
        settings = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
        // タイトル・タブ名を言語切り替えに対応
        titleContent = new GUIContent(T("exclusionListEditorWindow"));
    }

    /// <summary>
    /// 除外リスト編集用GUIの描画
    /// </summary>
    private void OnGUI()
    {
        // 設定が見つからない場合は警告表示
        if (settings == null)
        {
            GUILayout.Label(T("settingsNotFound"));
            return;
        }

        titleContent = new GUIContent(T("exclusionListEditorWindow"));
        scroll = EditorGUILayout.BeginScrollView(scroll);

        // 除外フォルダリストの編集
        GUILayout.Label(T("excludedFolders"), EditorStyles.boldLabel);
        for (int i = 0; i < settings.excludedFolders.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            string prev = settings.excludedFolders[i];
            string next = EditorGUILayout.TextField(prev);
            if (!IsValidFolder(next))
            {
                EditorGUILayout.LabelField("無効なフォルダ名", EditorStyles.miniLabel, GUILayout.Width(100));
            }
            settings.excludedFolders[i] = next;
            if (GUILayout.Button(T("remove"), GUILayout.Width(50)))
            {
                settings.excludedFolders.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        // フォルダ追加ボタン（空欄禁止）
        if (GUILayout.Button(T("addFolder")))
        {
            string newFolder = "";
            if (!IsValidFolder(newFolder))
            {
                EditorUtility.DisplayDialog("入力エラー", "空欄のフォルダは追加できません。", "OK");
            }
            else
            {
                settings.excludedFolders.Add(newFolder);
            }
        }

        GUILayout.Space(10);

        // 除外拡張子リストの編集
        GUILayout.Label(T("excludedExtensions"), EditorStyles.boldLabel);
        for (int i = 0; i < settings.excludedExtensions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            string prev = settings.excludedExtensions[i];
            string next = EditorGUILayout.TextField(prev);
            if (!IsValidExtension(next))
            {
                EditorGUILayout.LabelField("無効な拡張子", EditorStyles.miniLabel, GUILayout.Width(100));
            }
            settings.excludedExtensions[i] = next;
            if (GUILayout.Button(T("remove"), GUILayout.Width(50)))
            {
                settings.excludedExtensions.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        // 拡張子追加ボタン（空欄禁止）
        if (GUILayout.Button(T("addExtension")))
        {
            string newExt = "";
            if (!IsValidExtension(newExt))
            {
                EditorUtility.DisplayDialog("入力エラー", "空欄の拡張子は追加できません。", "OK");
            }
            else
            {
                settings.excludedExtensions.Add(newExt);
            }
        }

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
}