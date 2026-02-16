using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

/// <summary>
/// 未使用アセットを検出・管理・削除するためのEditorウィンドウ
/// </summary>
public class UnusedAssetCleanerWindow : EditorWindow
{
    /// <summary>
    /// サポートする言語
    /// </summary>
    public enum Language { Japanese, English, ChineseSimplified, ChineseTraditional }
    private Language currentLanguage = Language.Japanese;

    private const string CleanerSettingsPath = "Assets/Editor/CleanerSettings.asset";
    private const string TrashFolder = "Assets/Trash";
    private const string ReportFileName = "UnusedAssetsReport.txt";
    private const string DisplayFilterSettingsPath = "Assets/Editor/DisplayFilterSettings.asset";

    /// <summary>
    /// UIテキストの多言語辞書
    /// </summary>
    private static Dictionary<string, (string ja, string en, string zh, string zht)> StaticTexts = new Dictionary<string, (string ja, string en, string zh, string zht)>
    {
        ["title"] = ("Unused Asset Cleaner", "Unused Asset Cleaner", "未使用资源清理器", "未使用資源清理器"),
        ["scan"] = ("未使用アセットスキャン", "Scan Unused Assets", "扫描未使用资源", "掃描未使用資源"),
        ["scanStart"] = ("スキャン開始...", "Scan started...", "开始扫描...", "開始掃描..."),
        ["filter"] = ("表示フィルター", "Display Filter", "显示过滤器", "顯示過濾器"),
        ["showSettings"] = ("Assets/Settingsを表示", "Show Assets/Settings", "显示Assets/Settings", "顯示Assets/Settings"),
        ["showEditor"] = ("Assets/Editorを表示", "Show Assets/Editor", "显示Assets/Editor", "顯示Assets/Editor"),
        ["showReadme"] = ("Assets/Readme.assetを表示", "Show Assets/Readme.asset", "显示Assets/Readme.asset", "顯示Assets/Readme.asset"),
        ["showResources"] = ("Assets/Resourcesを表示", "Show Assets/Resources", "显示Assets/Resources", "顯示Assets/Resources"),
        ["showPlugins"] = ("Assets/Pluginsを表示", "Show Assets/Plugins", "显示Assets/Plugins", "顯示Assets/Plugins"),
        ["showStreamingAssets"] = ("Assets/StreamingAssetsを表示", "Show Assets/StreamingAssets", "显示Assets/StreamingAssets", "顯示Assets/StreamingAssets"),
        ["showAddressableAssets"] = ("Assets/AddressableAssetsを表示", "Show Assets/AddressableAssets", "显示Assets/AddressableAssets", "顯示Assets/AddressableAssets"),
        ["editExclusion"] = ("除外リスト編集ウィンドウを開く", "Open Exclusion List Editor", "打开排除列表编辑器", "打開排除列表編輯器"),
        ["detected"] = ("検出数", "Detected", "检测到", "檢測到"),
        ["totalSize"] = ("合計サイズ", "Total Size", "总大小", "總大小"),
        ["search"] = ("検索", "Search", "搜索", "搜尋"),
        ["exclude"] = ("除外", "Exclude", "排除", "排除"),
        ["details"] = ("詳細", "Details", "详情", "詳情"),
        ["moveToTrash"] = ("選択したアセットをゴミ箱へ移動", "Move Selected Assets to Trash", "将选中的资源移至回收站", "將選中的資源移至回收站"),
        ["confirmMove"] = ("選択したアセットをゴミ箱へ移動しますか？", "Move selected assets to Trash?", "是否将选中的资源移至回收站？", "是否將選中的資源移至回收站？"),
        ["yes"] = ("はい", "Yes", "是", "是"),
        ["cancel"] = ("キャンセル", "Cancel", "取消", "取消"),
        ["excludeAll"] = ("選択したアセットを一括除外", "Exclude Selected Assets", "批量排除选中的资源", "批量排除選中的資源"),
        ["trashList"] = ("ゴミ箱内アセット一覧", "Assets in Trash", "回收站中的资源列表", "回收站中的資源列表"),
        ["reloadTrash"] = ("ゴミ箱を再読込", "Reload Trash", "重新加载回收站", "重新加載回收站"),
        ["trashEmpty"] = ("ゴミ箱は空です。", "Trash is empty.", "回收站为空。", "回收站為空。"),
        ["restore"] = ("選択したアセットを復元", "Restore Selected Assets", "恢复选中的资源", "恢復選中的資源"),
        ["confirmRestore"] = ("選択したアセットを復元しますか？", "Restore selected assets?", "是否恢复选中的资源？", "是否恢復選中的資源？"),
        ["delete"] = ("選択したアセットを完全削除", "Delete Selected Assets Permanently", "永久删除选中的资源", "永久刪除選中的資源"),
        ["confirmDelete"] = ("選択したアセットを完全に削除します。元に戻せません。", "Delete selected assets permanently. This cannot be undone.", "永久删除选中的资源。此操作不可撤销。", "永久刪除選中的資源。此操作不可復原。"),
        ["logReport"] = ("ログ・レポート", "Log & Report", "日志与报告", "日誌與報告"),
        ["clearLog"] = ("ログをクリア", "Clear Log", "清除日志", "清除日誌"),
        ["logOutput"] = ("Log Output:", "Log Output:", "日志输出：", "日誌輸出："),
        ["reportDiff"] = ("レポート・差分機能", "Report & Diff", "报道与差异", "報告與差異"),
        ["outputReport"] = ("未使用アセットレポートを出力", "Output Unused Asset Report", "导出未使用资源报告", "匯出未使用資源報告"),
        ["prevReport"] = ("前回レポートファイル", "Previous Report File", "上次报告文件", "上次報告文件"),
        ["diffCompare"] = ("前回レポートと差分比較", "Compare with Previous Report", "与上次报告进行差异比较", "與上次報告進行差異比較"),
        ["diffResult"] = ("差分結果", "Diff Result", "差异结果", "差異結果"),
        ["undo"] = ("直前の操作を取り消す", "Undo Last Action", "撤销上一步操作", "復原上一步操作"),
        ["redo"] = ("取り消した操作を再度実行", "Redo Last Action", "重做撤销的操作", "重做復原的操作"),
        ["instanceId"] = ("InstanceID:", "InstanceID:", "实例ID：", "實例ID："),
        ["assetDetails"] = ("アセット詳細", "Asset Details", "资源详情", "資源詳情"),
        ["path"] = ("パス", "Path", "路径", "路徑"),
        ["size"] = ("サイズ", "Size", "大小", "大小"),
        ["type"] = ("型", "Type", "类型", "類型"),
        ["dependencies"] = ("参照元", "Dependencies", "依赖项", "依賴項"),
        ["dragDrop"] = ("ここにレポートファイル（.txt）をドラッグ＆ドロップ", "Drag & drop report file (.txt) here", "将报告文件（.txt）拖放到此处", "將報告文件（.txt）拖放到此处"),
        ["moveToTrashLog"] = ("ゴミ箱へ移動: {0}件", "Moved to Trash: {0} assets", "移至回收站：{0}项", "移至回收站：{0}項"),
        ["excludeAllLog"] = ("一括除外: {0}件", "Excluded: {0} assets", "批量排除：{0}项", "批量排除：{0}項"),
        ["outputReportLog"] = ("レポート出力: {0} ({1}件)", "Report output: {0} ({1} assets)", "报告导出：{0}（{1}项）", "報告匯出：{0}（{1}項）"),
        ["restoreLog"] = ("復元: {0}件", "Restored: {0} assets", "已恢复：{0}项", "已恢復：{0}項"),
        ["deleteLog"] = ("完全削除: {0}件", "Deleted: {0} assets", "已永久删除：{0}项", "已永久刪除：{0}項"),
        ["diffCompleteLog"] = ("差分比較完了", "Diff comparison complete", "差异比较完成", "差異比較完成"),
        ["scanCompleteLog"] = ("完了: 未使用アセット数: {0}", "Complete: Unused asset count: {0}", "完成：未使用资源数：{0}", "完成：未使用資源數：{0}"),
        ["diffAdded"] = ("追加: {0}件", "Added: {0} assets", "新增：{0}项", "新增：{0}項"),
        ["diffRemoved"] = ("削除: {0}件", "Removed: {0} assets", "移除：{0}项", "移除：{0}項"),
        ["trashCount"] = ("ゴミ箱内: {0}件", "In Trash: {0} assets", "回收站中：{0}项", "回收站中：{0}項"),
        ["openReportFolder"] = ("レポートフォルダを開く", "Open Report Folder", "打开报告文件夹", "打開報告資料夾"),
        ["language"] = ("言語：", "Language:", "语言：", "語言："),
        ["settingsNotFound"] = (
            "CleanerSettingsが見つかりません。",
            "CleanerSettings not found.",
            "未找到CleanerSettings。",
            "未找到CleanerSettings。"
        ),
        ["excludedFolders"] = (
            "除外フォルダ",
            "Excluded Folders",
            "排除文件夹",
            "排除資料夾"
        ),
        ["remove"] = (
            "削除",
            "Remove",
            "移除",
            "移除"
        ),
        ["addFolder"] = (
            "フォルダ追加",
            "Add Folder",
            "添加文件夹",
            "新增資料夾"
        ),
        ["excludedExtensions"] = (
            "除外拡張子",
            "Excluded Extensions",
            "排除扩展名",
            "排除副檔名"
        ),
        ["addExtension"] = (
            "拡張子追加",
            "Add Extension",
            "添加扩展名",
            "新增副檔名"
        ),
        ["selectGroupAll"] = (
            "このグループ内のアセットをすべて選択",
            "Select all assets in this group",
            "选择此分组中的所有资源",
            "選擇此分組中的所有資源"
        ),
        // 表示対象フィルター用
        ["includeFolders"] = ("表示対象フォルダ", "Include Folders", "显示目标文件夹", "顯示目標資料夾"),
        ["includeExtensions"] = ("表示対象拡張子", "Include Extensions", "显示目标扩展名", "顯示目標副檔名"),
        ["onlyShowFolder"] = ("{0} のみ表示", "Show only {0}", "仅显示 {0}", "僅顯示 {0}"),
        ["onlyShowExtension"] = ("{0} のみ表示", "Show only {0}", "仅显示 {0}", "僅顯示 {0}"),
        ["editDisplayFilter"] = (
            "表示フィルター編集ウィンドウを開く",
            "Open Display Filter Window",
            "打开显示过滤器窗口",
            "打開顯示過濾器視窗"
        ),
        ["exclusionListEditorWindow"] = (
            "除外リスト編集ウィンドウ",
            "Exclusion List Editor Window",
            "排除列表编辑窗口",
            "排除列表編輯視窗"
        ),
        ["addressablesWarning"] = (
            "Addressables機能が有効になっていません。Player Settings > Scripting Define Symbols で 'UNITY_ADDRESSABLES' を追加してください。",
            "Addressables is not enabled. Please add 'UNITY_ADDRESSABLES' to Scripting Define Symbols in Player Settings.",
            "未启用Addressables。请在Player Settings的Scripting Define Symbols中添加'UNITY_ADDRESSABLES'。",
            "未啟用Addressables。請在Player Settings的Scripting Define Symbols中添加'UNITY_ADDRESSABLES'。"
        ),
    };

    /// <summary>
    /// 検出された未使用アセットのパス一覧（除外対象も含む）
    /// </summary>
    private List<string> unusedAssets = new List<string>();
    /// <summary>
    /// 表示対象の未使用アセット（除外対象は含まない）
    /// </summary>
    private List<string> filteredAssets = new List<string>();
    /// <summary>
    /// アセットごとの選択状態（チェックボックス用）
    /// </summary>
    private List<bool> assetSelections = new List<bool>();
    private Vector2 scrollPos;
    /// <summary>
    /// 操作ログ出力
    /// </summary>
    private string logOutput = "";

    /// <summary>
    /// 未使用アセットの合計サイズ
    /// </summary>
    private long totalSize = 0;
    /// <summary>
    /// スキャン中フラグ
    /// </summary>
    private bool isScanning = false;
    /// <summary>
    /// スキャン進捗（0.0～1.0）
    /// </summary>
    private float scanProgress = 0f;
    /// <summary>
    /// スキャン進捗メッセージ
    /// </summary>
    private string scanMessage = "";

    /// <summary>
    /// アセットグループごとの折りたたみ状態
    /// </summary>
    private Dictionary<string, bool> groupFoldouts = new Dictionary<string, bool>();

    // 表示フィルター（各種アセット種別の表示切替）
    private bool showAssetsSettings = false;
    private bool showAssetsEditor = false;
    private bool showReadmeAsset = false;
    private bool showResources = false;
    private bool showPlugins = false;
    private bool showStreamingAssets = false;
    private bool showAddressableAssets = false;

    /// <summary>
    /// ゴミ箱内のアセット一覧
    /// </summary>
    private List<(int instanceId, string assetPath)> trashAssets = new List<(int, string)>();
    /// <summary>
    /// ゴミ箱アセットごとの選択状態
    /// </summary>
    private List<bool> trashSelections = new List<bool>();

    /// <summary>
    /// レポート出力先パス
    /// </summary>
    private string reportPath = ReportFileName;
    /// <summary>
    /// 差分比較用の前回レポートパス
    /// </summary>
    private string previousReportPath = "";
    /// <summary>
    /// 差分結果表示用
    /// </summary>
    private string diffResult = "";

    /// <summary>
    /// アセット検索用テキスト
    /// </summary>
    private string searchText = "";

    private const string LanguagePrefKey = "UnusedAssetCleaner_Language";

    /// <summary>
    /// 現在の言語（静的参照用）
    /// </summary>
    public static Language CurrentLanguageStatic { get; private set; }

    // --- 表示対象フィルター用変数 ---
    private HashSet<string> includeFolders = new HashSet<string>();
    private HashSet<string> includeExtensions = new HashSet<string>();

    private DisplayFilterSettings displayFilterSettings;

    // --- 改善点用変数 ---
    private bool showRestartNotice = false; // ゴミ箱移動後の再起動案内
    private bool showAddressablesWarning = false; // Addressables未設定時の警告

    /// <summary>
    /// ウィンドウ有効化時の初期化処理
    /// </summary>
    private void OnEnable()
    {
#if !UNITY_ADDRESSABLES
        showAddressablesWarning = true;
#endif
        currentLanguage = (Language)EditorPrefs.GetInt(LanguagePrefKey, (int)Language.Japanese);
        CurrentLanguageStatic = currentLanguage;
        EnsureCleanerSettingsExists();
        ReloadTrashAssets();
        UpdateFolderFilterStates();
        UpdateExtensionFilterStates();

        displayFilterSettings = AssetDatabase.LoadAssetAtPath<DisplayFilterSettings>(DisplayFilterSettingsPath);
        if (displayFilterSettings == null)
        {
            displayFilterSettings = ScriptableObject.CreateInstance<DisplayFilterSettings>();
            AssetDatabase.CreateAsset(displayFilterSettings, DisplayFilterSettingsPath);
            AssetDatabase.SaveAssets();
        }
    }

    // 2. 除外リストの変更を検知してフィルターリストを再構築
    private void UpdateFolderFilterStates()
    {
        var settings = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
        if (settings == null) return;

        // 既存の状態を保持しつつ、除外リストに合わせて更新
        var newStates = new Dictionary<string, bool>();
        foreach (var folder in settings.excludedFolders)
        {
            if (folderFilterStates.ContainsKey(folder))
                newStates[folder] = folderFilterStates[folder];
            else
                newStates[folder] = false; // デフォルトは非表示
        }
        folderFilterStates = newStates;
    }

    private void UpdateExtensionFilterStates()
    {
        var settings = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
        if (settings == null) return;
        // 状態の初期化のみ行う
        var newStates = new Dictionary<string, bool>();
        foreach (var ext in settings.excludedExtensions)
        {
            bool prev = extensionFilterStates.ContainsKey(ext) ? extensionFilterStates[ext] : false;
            newStates[ext] = prev;
        }
        extensionFilterStates = newStates;
    }

    /// <summary>
    /// CleanerSettings.assetが存在しない場合は自動生成
    /// </summary>
    private void EnsureCleanerSettingsExists()
    {
        var settings = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
        if (settings == null)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                AssetDatabase.CreateFolder("Assets", "Editor");
            settings = ScriptableObject.CreateInstance<CleanerSettings>();
            AssetDatabase.CreateAsset(settings, CleanerSettingsPath);
            AssetDatabase.SaveAssets();
            Debug.Log("CleanerSettings.asset を自動生成しました。");
        }
    }

    /// <summary>
    /// 指定キーの翻訳テキストを取得する関数（静的）
    /// </summary>
    public static Func<string, string> GetTranslation
    {
        get
        {
            return key =>
            {
                if (StaticTexts.TryGetValue(key, out var quad))
                {
                    return CurrentLanguageStatic switch
                    {
                        Language.Japanese => quad.ja,
                        Language.English => quad.en,
                        Language.ChineseSimplified => quad.zh,
                        Language.ChineseTraditional => quad.zht,
                        _ => key
                    };
                }
                Debug.LogWarning($"[UnusedAssetCleaner] ローカライズ辞書に未定義キー: {key}");
                return key;
            };
        }
    }

    /// <summary>
    /// メニューからウィンドウを表示
    /// </summary>
    [MenuItem("Tools/Unused Asset Cleaner")]
    public static void ShowWindow()
    {
        GetWindow<UnusedAssetCleanerWindow>("Unused Asset Cleaner");
    }

    /// <summary>
    /// メインGUI描画処理
    /// </summary>
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        {
            GUILayout.Space(10);
            // タイトル表示
            GUILayout.Label(T("title"), new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 20 });
            GUILayout.Space(10);

            // Addressables未設定時の警告表示
            if (showAddressablesWarning)
            {
                EditorGUILayout.HelpBox(T("addressablesWarning"), MessageType.Warning);
            }

            // 言語選択UI
            GUILayout.BeginHorizontal();
            GUILayout.Label(T("language"), GUILayout.Width(70));
            string[] langs = { "日本語", "English", "简体中文", "繁體中文" };
            int selected = (int)currentLanguage;
            selected = EditorGUILayout.Popup(selected, langs, GUILayout.Width(100));
            if ((int)currentLanguage != selected)
            {
                currentLanguage = (Language)selected;
                CurrentLanguageStatic = currentLanguage;
                EditorPrefs.SetInt(LanguagePrefKey, selected);
            }
            GUILayout.EndHorizontal();

            // 未使用アセットスキャンUI
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(T("scan"), EditorStyles.boldLabel);
            if (isScanning)
            {
                // スキャン進捗バー
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 20), scanProgress, scanMessage);
                GUILayout.Space(10);
            }
            if (GUILayout.Button(T("scan"), GUILayout.Height(30)) && !isScanning)
            {
                unusedAssets.Clear();
                assetSelections.Clear();
                totalSize = 0;
                logOutput = "";
                isScanning = true;
                scanProgress = 0f;
                scanMessage = T("scanStart");

                var scannedAssets = AssetScanner.ScanUnusedAssets(
                    (progress, message) =>
                    {
                        scanProgress = progress;
                        scanMessage = message;
                        Repaint();
                        EditorUtility.DisplayProgressBar("Unused Asset Scan", message, progress);
                    },
                    null
                );

                unusedAssets = scannedAssets
                    .Where(path => !path.StartsWith(TrashFolder + "/") && !path.Equals(TrashFolder))
                    .ToList();

                UpdateFilteredAssets();

                assetSelections = new List<bool>(new bool[filteredAssets.Count]);
                totalSize = 0;
                foreach (var assetPath in unusedAssets)
                {
                    string absolutePath = GetAbsolutePath(assetPath);
                    try
                    {
                        if (!string.IsNullOrEmpty(absolutePath) && File.Exists(absolutePath))
                            totalSize += new FileInfo(absolutePath).Length;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"ファイルサイズ取得失敗: {absolutePath} : {ex.Message}");
                    }
                }

                groupFoldouts.Clear();
                foreach (var group in filteredAssets.Select(GetAssetGroup).Distinct())
                    groupFoldouts[group] = true;

                groupSelections.Clear();
                foreach (var group in filteredAssets.Select(GetAssetGroup).Distinct())
                    groupSelections[group] = false;

                logOutput = string.Format(T("scanCompleteLog"), unusedAssets.Count);
                isScanning = false;
                EditorUtility.ClearProgressBar();
                Repaint();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // 除外リスト編集ウィンドウ表示ボタン
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(T("editExclusion"), GUILayout.Width(200)))
            {
                ExcludedListEditorWindow.ShowWindow();
                UpdateFolderFilterStates();
                UpdateFilteredAssets();
                Repaint();
            }
            if (GUILayout.Button(T("editDisplayFilter"), GUILayout.Width(200)))
            {
                DisplayFilterListEditorWindow.ShowWindow();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 除外フォルダ・拡張子設定UI
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(T("excludedFolders"), EditorStyles.boldLabel);

            var settings = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
            if (settings != null)
            {
                for (int i = 0; i < settings.excludedFolders.Count; i++)
                {
                    string folder = settings.excludedFolders[i];
                    bool prev = folderFilterStates.ContainsKey(folder) ? folderFilterStates[folder] : false;
                    bool next = EditorGUILayout.ToggleLeft(folder, prev);
                    folderFilterStates[folder] = next;
                }
            }

            GUILayout.Space(5);
            GUILayout.Label(T("excludedExtensions"), EditorStyles.boldLabel);

            if (settings != null)
            {
                for (int i = 0; i < settings.excludedExtensions.Count; i++)
                {
                    string ext = settings.excludedExtensions[i];
                    bool prev = extensionFilterStates.ContainsKey(ext) ? extensionFilterStates[ext] : false;
                    bool next = EditorGUILayout.ToggleLeft(ext, prev);
                    extensionFilterStates[ext] = next;
                }
            }
            EditorGUILayout.EndVertical();

            // --- 表示対象フィルターUI ---
            // フォルダー・拡張子の表示名を正式名称（パスや拡張子）のみで表示

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(T("includeFolders"), EditorStyles.boldLabel);

            if (settings != null && displayFilterSettings != null)
            {
                for (int i = 0; i < displayFilterSettings.includeFolders.Count; i++)
                {
                    string folder = displayFilterSettings.includeFolders[i];
                    bool isIncludedFolder = includeFolders.Contains(folder);

                    // 正式名称のみ表示
                    string label = folder;

                    bool next = EditorGUILayout.ToggleLeft(label, isIncludedFolder);
                    if (next)
                        includeFolders.Add(folder);
                    else
                        includeFolders.Remove(folder);
                }
                // 空欄追加禁止
                if (GUILayout.Button(T("addFolder")))
                {
                    string newFolder = "";
                    if (string.IsNullOrWhiteSpace(newFolder))
                    {
                        EditorUtility.DisplayDialog("入力エラー", "空欄のフォルダは追加できません。", "OK");
                    }
                    else
                    {
                        displayFilterSettings.includeFolders.Add(newFolder);
                    }
                }
            }

            GUILayout.Space(5);
            GUILayout.Label(T("includeExtensions"), EditorStyles.boldLabel);

            if (settings != null && displayFilterSettings != null)
            {
                for (int i = 0; i < displayFilterSettings.includeExtensions.Count; i++)
                {
                    string ext = displayFilterSettings.includeExtensions[i];
                    bool isIncludedExt = includeExtensions.Contains(ext);

                    // 正式名称のみ表示
                    string label = ext;

                    bool next = EditorGUILayout.ToggleLeft(label, isIncludedExt);
                    if (next)
                        includeExtensions.Add(ext);
                    else
                        includeExtensions.Remove(ext);
                }
                // 空欄追加禁止
                if (GUILayout.Button(T("addExtension")))
                {
                    string newExt = "";
                    if (string.IsNullOrWhiteSpace(newExt))
                    {
                        EditorUtility.DisplayDialog("入力エラー", "空欄の拡張子は追加できません。", "OK");
                    }
                    else
                    {
                        displayFilterSettings.includeExtensions.Add(newExt);
                    }
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            if (unusedAssets.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label($"{T("detected")}: {unusedAssets.Count}　{T("totalSize")}: {FormatBytes(totalSize)}", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16 });

                searchText = EditorGUILayout.TextField(T("search"), searchText);

                UpdateFilteredAssets();

                var grouped = filteredAssets
                    .Select((path, idx) => new { path, idx, group = GetAssetGroup(path), isReadme = Path.GetFileName(path).Equals("README.asset", System.StringComparison.OrdinalIgnoreCase) })
                    .GroupBy(x => x.group);

                foreach (var group in grouped)
                {
                    // 表示フィルター判定
                    if ((group.Key == "Settings" && !showAssetsSettings) ||
                        (group.Key == "Editor" && !showAssetsEditor) ||
                        (group.Key == "Resources" && !showResources) ||
                        (group.Key == "Plugins" && !showPlugins) ||
                        (group.Key == "StreamingAssets" && !showStreamingAssets) ||
                        (group.Key == "AddressableAssets" && !showAddressableAssets))
                        continue;

                    // README.asset表示判定
                    var visibleAssets = group.Where(asset => !(asset.isReadme && !showReadmeAsset)).ToList();
                    if (visibleAssets.Count == 0)
                        continue;

                    // グループ折りたたみUI
                    groupFoldouts[group.Key] = EditorGUILayout.Foldout(groupFoldouts.ContainsKey(group.Key) ? groupFoldouts[group.Key] : true, group.Key, true);

                    if (groupFoldouts[group.Key])
                    {
                        EditorGUILayout.BeginVertical("box");

                        // グループ全選択チェックボックス
                        bool allSelected = group.All(asset => assetSelections[asset.idx]);
                        bool prevGroupSelected = groupSelections.ContainsKey(group.Key) ? groupSelections[group.Key] : false;
                        bool newGroupSelected = EditorGUILayout.ToggleLeft(T("selectGroupAll"), allSelected);

                        // グループ全選択チェックボックス操作時
                        if (newGroupSelected != allSelected)
                        {
                            groupSelections[group.Key] = newGroupSelected;
                            foreach (var asset in group)
                            {
                                assetSelections[asset.idx] = newGroupSelected;
                            }
                        }

                        // 個別チェックボックス描画
                        foreach (var asset in visibleAssets)
                        {
                            EditorGUILayout.BeginHorizontal();
                            bool prev = assetSelections[asset.idx];
                            assetSelections[asset.idx] = EditorGUILayout.Toggle(assetSelections[asset.idx], GUILayout.Width(20));
                            var type = AssetDatabase.GetMainAssetTypeAtPath(asset.path);
                            var iconContent = EditorGUIUtility.ObjectContent(null, type);
                            GUILayout.Label(iconContent.image, GUILayout.Width(18), GUILayout.Height(18));
                            EditorGUILayout.LabelField(asset.path); // 幅指定を外す
                            UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(asset.path);
                            int instanceId = obj != null ? obj.GetInstanceID() : -1;

                            GUILayout.Space(40);

                            // InstanceID表示
                            GUILayout.BeginVertical(GUILayout.Width(120));
                            GUILayout.Label(T("instanceId"), new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter });
                            GUILayout.Label(instanceId.ToString(), new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter });
                            GUILayout.EndVertical();

                            GUILayout.FlexibleSpace(); // 余白を自動調整

                            // 除外ボタン
                            if (GUILayout.Button(T("exclude"), GUILayout.Width(60)))
                            {
                                EnsureCleanerSettingsExists();
                                var settings2 = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
                                if (settings2 != null)
                                {
                                    if (Directory.Exists(asset.path))
                                    {
                                        settings2.excludedFolders.Add(asset.path);
                                    }
                                    else
                                    {
                                        string ext = Path.GetExtension(asset.path);
                                        if (!settings2.excludedExtensions.Contains(ext))
                                            settings2.excludedExtensions.Add(ext);
                                    }
                                    EditorUtility.SetDirty(settings2);
                                    AssetDatabase.SaveAssets();
                                    unusedAssets = AssetScanner.ScanUnusedAssets();
                                    UpdateFilteredAssets();
                                    assetSelections = new List<bool>(new bool[filteredAssets.Count]);
                                    Repaint();
                                }
                            }
                            // 詳細ボタン
                            if (GUILayout.Button(T("details"), GUILayout.Width(60)))
                            {
                                // アセット詳細情報ダイアログ表示
                                string info = $"{T("path")}: {asset.path}\n";
                                string absolutePath = GetAbsolutePath(asset.path);
                                try
                                {
                                    if (!string.IsNullOrEmpty(absolutePath) && File.Exists(absolutePath))
                                        info += $"{T("size")}: {new FileInfo(absolutePath).Length} bytes\n";
                                }
                                catch (Exception ex)
                                {
                                    info += $"{T("size")}: 取得失敗 ({ex.Message})\n";
                                }
                                info += $"{T("type")}: {type?.Name}\n";
                                var deps = AssetDatabase.GetDependencies(asset.path, false);
                                info += $"{T("dependencies")}: {string.Join(", ", deps)}";
                                EditorUtility.DisplayDialog(T("assetDetails"), info, "OK");
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        // 個別チェックボックスの変更を反映してグループ全選択状態を更新
                        groupSelections[group.Key] = group.All(asset => assetSelections[asset.idx]);

                        EditorGUILayout.EndVertical();
                    }
                }

                GUILayout.Space(10);

                // 一括操作ボタン（ゴミ箱移動・除外）
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(T("moveToTrash"), GUILayout.Height(30)))
                {
                    // 選択アセットをゴミ箱へまとめて移動
                    if (EditorUtility.DisplayDialog(T("confirmMove"), "", T("yes"), T("cancel")))
                    {
                        var selectedAssets = filteredAssets
                            .Where((path, idx) => assetSelections[idx])
                            .ToList();
                        TrashManager.MoveAllToTrash(selectedAssets);
                        showRestartNotice = true;
                    }
                }
                if (GUILayout.Button(T("excludeAll"), GUILayout.Height(30)))
                {
                    EnsureCleanerSettingsExists();
                    var settings2 = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
                    if (settings2 != null)
                    {
                        int excludeCount = 0;
                        for (int i = assetSelections.Count - 1; i >= 0; i--)
                        {
                            if (assetSelections[i])
                            {
                                string path = filteredAssets[i];
                                if (Directory.Exists(path))
                                {
                                    settings2.excludedFolders.Add(path);
                                }
                                else
                                {
                                    string ext = Path.GetExtension(path);
                                    if (!settings2.excludedExtensions.Contains(ext))
                                        settings2.excludedExtensions.Add(ext);
                                }
                                unusedAssets.Remove(path);
                                filteredAssets.RemoveAt(i);
                                assetSelections.RemoveAt(i);
                                excludeCount++;
                            }
                        }
                        EditorUtility.SetDirty(settings2);
                        AssetDatabase.SaveAssets();
                        logOutput = string.Format(T("excludeAllLog"), excludeCount);
                        UpdateFilteredAssets();
                        assetSelections = new List<bool>(new bool[filteredAssets.Count]);
                        Repaint();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // ゴミ箱移動後の再起動案内
                if (showRestartNotice)
                {
                    EditorGUILayout.HelpBox("ゴミ箱へ移動後、Unityが自動的に再起動されます。再度ツールを開いてください。", MessageType.Info);
                }

                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(10);

            // ゴミ箱一覧表示
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(T("trashList"), EditorStyles.boldLabel);
            if (GUILayout.Button(T("reloadTrash"), GUILayout.Width(120)))
            {
                ReloadTrashAssets();
            }
            if (trashAssets.Count == 0)
            {
                ReloadTrashAssets();
                GUILayout.Label(T("trashEmpty"), EditorStyles.miniLabel);
            }
            else
            {
                GUILayout.Label(string.Format(T("trashCount"), trashAssets.Count), EditorStyles.miniBoldLabel);
                for (int i = 0; i < trashAssets.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    trashSelections[i] = EditorGUILayout.Toggle(trashSelections[i], GUILayout.Width(20));
                    var type = AssetDatabase.GetMainAssetTypeAtPath(trashAssets[i].assetPath);
                    var iconContent = EditorGUIUtility.ObjectContent(null, type);
                    var preview = AssetPreview.GetAssetPreview(AssetDatabase.LoadMainAssetAtPath(trashAssets[i].assetPath));
                    if (preview != null)
                        GUILayout.Label(preview, GUILayout.Width(32), GUILayout.Height(32));
                    else
                        GUILayout.Label(iconContent.image, GUILayout.Width(18), GUILayout.Height(18));
                    EditorGUILayout.LabelField(Path.GetFileName(trashAssets[i].assetPath));
                    GUILayout.Label($"ID: {trashAssets[i].instanceId}", GUILayout.Width(80));
                    EditorGUILayout.EndHorizontal();
                }
            }
            // Trash操作ボタン（復元・削除）
            EditorGUILayout.BeginHorizontal();
            if (trashAssets.Count > 0 && GUILayout.Button(T("restore"), GUILayout.Height(30)))
            {
                int restoreCount = 0;
                for (int i = trashSelections.Count - 1; i >= 0; i--)
                {
                    if (trashSelections[i])
                    {
                        var (instanceId, assetPath) = trashAssets[i];
                        string fileName = Path.GetFileName(assetPath);
                        string originalPath = $"Assets/{fileName}";
                        if (TrashManager.RestoreFromTrash(fileName, originalPath))
                        {
                            trashAssets.RemoveAt(i);
                            trashSelections.RemoveAt(i);
                            restoreCount++;
                        }
                    }
                }
                logOutput = string.Format(T("restoreLog"), restoreCount);
                Debug.Log(logOutput);
                ReloadTrashAssets();
            }
            if (trashAssets.Count > 0 && GUILayout.Button(T("delete"), GUILayout.Height(30)))
            {
                // 完全削除はUndoできない旨の警告
                if (EditorUtility.DisplayDialog("警告", "完全削除は元に戻せません。Undoはできません。本当に削除しますか？", "削除", "キャンセル"))
                {
                    int deleteCount = 0;
                    for (int i = trashSelections.Count - 1; i >= 0; i--)
                    {
                        if (trashSelections[i])
                        {
                            var (instanceId, assetPath) = trashAssets[i];
                            string fileName = Path.GetFileName(assetPath);
                            if (TrashManager.DeleteFromTrash(fileName))
                            {
                                trashAssets.RemoveAt(i);
                                trashSelections.RemoveAt(i);
                                deleteCount++;
                            }
                        }
                    }
                    logOutput = string.Format(T("deleteLog"), deleteCount);
                    Debug.Log(logOutput);
                    ReloadTrashAssets();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // ログ・レポート・差分機能
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(T("logReport"), EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(T("clearLog"), GUILayout.Width(120)))
            {
                logOutput = "";
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label(T("logOutput"), EditorStyles.boldLabel);
            GUILayout.TextArea(logOutput, GUILayout.Height(60));

            GUILayout.Space(10);
            GUILayout.Label(T("reportDiff"), EditorStyles.boldLabel);
            if (GUILayout.Button(T("outputReport"), GUILayout.Height(30)))
            {
                try
                {
                    File.WriteAllLines(reportPath, unusedAssets);
                    logOutput = string.Format(T("outputReportLog"), reportPath, unusedAssets.Count);
                    Debug.Log(logOutput);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"レポート出力失敗: {ex.Message}");
                    logOutput = $"レポート出力失敗: {ex.Message}";
                }
            }
            if (GUILayout.Button(T("openReportFolder"), GUILayout.Height(24)))
            {
                try
                {
                    string fullPath = Path.GetFullPath(reportPath);
                    EditorUtility.RevealInFinder(fullPath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"フォルダを開けません: {ex.Message}");
                }
            }

            previousReportPath = EditorGUILayout.TextField(T("prevReport"), previousReportPath);
            DrawDragDropField();
            if (GUILayout.Button(T("diffCompare"), GUILayout.Height(30)))
            {
                try
                {
                    var diff = AssetScanner.DiffUnusedAssets(previousReportPath);
                    diffResult = string.Format(T("diffAdded"), diff.added.Count) + "\n" +
                                 string.Join("\n", diff.added) +
                                 "\n" + string.Format(T("diffRemoved"), diff.removed.Count) + "\n" +
                                 string.Join("\n", diff.removed);
                    logOutput = T("diffCompleteLog");
                    Debug.Log(logOutput);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"差分比較失敗: {ex.Message}");
                    logOutput = $"差分比較失敗: {ex.Message}";
                }
            }
            if (!string.IsNullOrEmpty(diffResult))
            {
                GUILayout.Label(T("diffResult"), EditorStyles.boldLabel);
                GUILayout.TextArea(diffResult, GUILayout.Height(120));
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Undo/Redo操作
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(T("undo"), GUILayout.Width(120)))
                Undo.PerformUndo();
            if (GUILayout.Button(T("redo"), GUILayout.Width(145)))
                Undo.PerformRedo();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 表示対象の未使用アセットリストを更新
    /// 除外リストに該当するものは表示しない。表示フィルターON時のみ表示。
    /// </summary>
    private void UpdateFilteredAssets()
    {
        var settings = AssetDatabase.LoadAssetAtPath<CleanerSettings>(CleanerSettingsPath);
        filteredAssets = unusedAssets
            .Where(path =>
            {
                if (includeFolders.Count > 0)
                {
                    bool match = false;
                    foreach (var folder in includeFolders)
                    {
                        string folderNorm = folder.Replace("\\", "/");
                        if (path.StartsWith(folderNorm, StringComparison.OrdinalIgnoreCase) ||
                            path.Contains("/" + folderNorm))
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match) return false;
                }
                if (includeExtensions.Count > 0)
                {
                    string ext = System.IO.Path.GetExtension(path);
                    if (!includeExtensions.Contains(ext))
                        return false;
                }
                bool excludedByFolder = false;
                bool folderChecked = false;
                if (settings != null)
                {
                    foreach (var folder in settings.excludedFolders)
                    {
                        string folderNorm = folder.Replace("\\", "/");
                        if (path.StartsWith(folderNorm, StringComparison.OrdinalIgnoreCase) ||
                            path.Contains("/" + folderNorm))
                        {
                            excludedByFolder = true;
                            folderChecked = folderFilterStates.ContainsKey(folder) && folderFilterStates[folder];
                            break;
                        }
                    }
                }
                bool excludedByExt = false;
                bool extChecked = false;
                if (settings != null)
                {
                    string ext = Path.GetExtension(path);
                    if (settings.excludedExtensions.Contains(ext))
                    {
                        excludedByExt = true;
                        extChecked = extensionFilterStates.ContainsKey(ext) && extensionFilterStates[ext];
                    }
                }
                if (excludedByFolder && !folderChecked) return false;
                if (excludedByExt && !extChecked) return false;
                return string.IsNullOrEmpty(searchText) || path.Contains(searchText, StringComparison.OrdinalIgnoreCase);
            })
            .ToList();

        // assetSelectionsの数をfilteredAssetsに合わせて同期
        if (assetSelections == null || assetSelections.Count != filteredAssets.Count)
            assetSelections = new List<bool>(new bool[filteredAssets.Count]);
    }

    /// <summary>
    /// アセットパスを絶対パスに変換
    /// </summary>
    private string GetAbsolutePath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath)) return null;
        if (!assetPath.StartsWith("Assets/") && !assetPath.Equals("Assets")) return null;
        string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
        return Path.Combine(projectPath, assetPath.Replace("/", Path.DirectorySeparatorChar.ToString()));
    }

    /// <summary>
    /// ドラッグ＆ドロップ用フィールド描画
    /// </summary>
    private GUIStyle GetBoxStyle(int fontSize = 0, TextAnchor anchor = TextAnchor.MiddleLeft)
    {
        var style = new GUIStyle(GUI.skin.box);
        style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        style.alignment = anchor;
        if (fontSize > 0) style.fontSize = fontSize;
        return style;
    }

    /// <summary>
    /// レポートファイルのドラッグ＆ドロップ受付
    /// </summary>
    private void DrawDragDropField()
    {
        Rect dropArea = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, T("dragDrop"), GetBoxStyle(14, TextAnchor.MiddleCenter));

        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var path in DragAndDrop.paths)
                    {
                        if (Path.GetExtension(path).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            previousReportPath = path;
                            GUI.FocusControl(null);
                            break;
                        }
                    }
                    evt.Use();
                }
            }
        }
    }

    /// <summary>
    /// ゴミ箱内アセット一覧を再取得
    /// </summary>
    private void ReloadTrashAssets()
    {
        if (!AssetDatabase.IsValidFolder(TrashFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Trash");
            Debug.Log("Trashフォルダを自動生成しました。");
        }
        string trashAbsolutePath = GetAbsolutePath(TrashFolder);

        var files = Directory.Exists(trashAbsolutePath)
            ? Directory.GetFiles(trashAbsolutePath).Where(f => !f.EndsWith(".meta")).ToList()
            : new List<string>();
        var dirs = Directory.Exists(trashAbsolutePath)
            ? Directory.GetDirectories(trashAbsolutePath).ToList()
            : new List<string>();

        trashAssets = files
            .Select(f =>
            {
                string assetPath = "Assets/Trash/" + Path.GetFileName(f);
                var obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                int id = obj != null ? obj.GetInstanceID() : -1;
                return (id, assetPath);
            })
            .Concat(
                dirs.Select(d =>
                {
                    string assetPath = "Assets/Trash/" + Path.GetFileName(d);
                    var obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    int id = obj != null ? obj.GetInstanceID() : -1;
                    return (id, assetPath);
                })
            )
            .ToList();
        trashSelections = new List<bool>(new bool[trashAssets.Count]);
    }

    /// <summary>
    /// アセットパスからグループ名（第2階層）を取得
    /// </summary>
    private string GetAssetGroup(String assetPath)
    {
        var path = assetPath.Replace("\\", "/");
        var parts = path.Split('/');
        if (parts.Length >= 2)
            return parts[1];
        return "Root";
    }

    /// <summary>
    /// バイト数を人間が読みやすい単位に変換
    /// </summary>
    private string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024f:F2} KB";
        if (bytes < 1024 * 1024 * 1024) return $"{bytes / 1024f / 1024f:F2} MB";
        return $"{bytes / 1024f / 1024f / 1024f:F2} GB";
    }

    /// <summary>
    /// 指定キーのUIテキストを現在の言語で取得
    /// </summary>
    private string T(string key)
    {
        if (StaticTexts.TryGetValue(key, out var quad))
        {
            return currentLanguage switch
            {
                Language.Japanese => quad.ja,
                Language.English => quad.en,
                Language.ChineseSimplified => quad.zh,
                Language.ChineseTraditional => quad.zht,
                _ => key
            };
        }
        Debug.LogWarning($"[UnusedAssetCleaner] ローカライズ辞書に未定義キー: {key}");
        return key;
    }

    /// <summary>
    /// 現在の言語を取得（静的）
    /// </summary>
    public static Language GetCurrentLanguage()
    {
        return CurrentLanguageStatic;
    }

    private Dictionary<string, bool> groupSelections = new Dictionary<string, bool>();
    private Dictionary<string, bool> folderFilterStates = new Dictionary<string, bool>();
    private Dictionary<string, bool> extensionFilterStates = new Dictionary<string, bool>();
}