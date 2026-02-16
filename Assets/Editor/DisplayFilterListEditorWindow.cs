using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// DisplayFilterSettings の includeFolders / includeExtensions を編集するエディタウィンドウ。
/// </summary>
public class DisplayFilterListEditorWindow : EditorWindow
{
    private DisplayFilterSettings settings;
    private Vector2 scroll;

    private static System.Func<string, string> T =>
        UnusedAssetCleanerWindow.GetTranslation;

    private const string DisplayFilterSettingsPath = "Assets/Editor/DisplayFilterSettings.asset";

    // 入力バリデーション用
    private static bool IsValidFolder(string folder)
    {
        return !string.IsNullOrWhiteSpace(folder) &&
               !folder.Contains("\\") &&
               !folder.Contains("//") &&
               !folder.Contains("..") &&
               Regex.IsMatch(folder, @"^[\w\-/]+/?$");
    }
    private static bool IsValidExtension(string ext)
    {
        return !string.IsNullOrWhiteSpace(ext) &&
               ext.StartsWith(".") &&
               Regex.IsMatch(ext, @"^\.[a-zA-Z0-9]+$");
    }

    [MenuItem("Tools/表示フィルター編集")]
    public static void ShowWindow()
    {
        GetWindow<DisplayFilterListEditorWindow>(T("editDisplayFilter"));
    }

    private void OnEnable()
    {
        settings = AssetDatabase.LoadAssetAtPath<DisplayFilterSettings>(DisplayFilterSettingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<DisplayFilterSettings>();
            AssetDatabase.CreateAsset(settings, DisplayFilterSettingsPath);
            AssetDatabase.SaveAssets();
        }
    }

    private void OnGUI()
    {
        if (settings == null)
        {
            GUILayout.Label("DisplayFilterSettingsが見つかりません。");
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        // includeFolders 編集
        GUILayout.Label(T("includeFolders"), EditorStyles.boldLabel);
        for (int i = 0; i < settings.includeFolders.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            string prev = settings.includeFolders[i];
            string next = EditorGUILayout.TextField(prev);
            if (!IsValidFolder(next))
            {
                EditorGUILayout.LabelField("無効なフォルダ名", EditorStyles.miniLabel, GUILayout.Width(100));
            }
            settings.includeFolders[i] = next;
            if (GUILayout.Button(T("remove"), GUILayout.Width(50)))
            {
                settings.includeFolders.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button(T("addFolder")))
            settings.includeFolders.Add("");

        GUILayout.Space(10);

        // includeExtensions 編集
        GUILayout.Label(T("includeExtensions"), EditorStyles.boldLabel);
        for (int i = 0; i < settings.includeExtensions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            string prev = settings.includeExtensions[i];
            string next = EditorGUILayout.TextField(prev);
            if (!IsValidExtension(next))
            {
                EditorGUILayout.LabelField("無効な拡張子", EditorStyles.miniLabel, GUILayout.Width(100));
            }
            settings.includeExtensions[i] = next;
            if (GUILayout.Button(T("remove"), GUILayout.Width(50)))
            {
                settings.includeExtensions.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button(T("addExtension")))
            settings.includeExtensions.Add("");

        EditorGUILayout.EndScrollView();

        // 変更があれば保存
        if (GUI.changed)
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
}