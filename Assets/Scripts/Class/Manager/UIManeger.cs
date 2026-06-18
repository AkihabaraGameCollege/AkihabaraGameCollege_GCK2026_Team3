using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManeger : MonoBehaviour
{
    [SerializeField] GameObject Setting; // 設定画面のGameObject
    [SerializeField] GameObject Operating; // 操作説明画面のGameObject
    [SerializeField] GameObject StageSelect; // ステージ選択画面のGameObject
    [SerializeField] GameObject SoundSetting; // サウンド設定画面のGameObject
    [SerializeField] private UI_Anim uI_Anim; // ゲームオーバーUIのゲームオブジェクト
    [SerializeField] private DeadArea deadArea; // PlayerはDeadAreaを当たったか確認
    [SerializeField] public GameObject PauseUI; // ゲームオーバーUIのゲームオブジェクト
    [SerializeField] private List<GameObject> returnToCheckpointButtons; // チェックポイントに戻るボタンのリスト
    private SoundManager soundManager; // SoundManagerのインスタンス
    private GameDirectory gameDirectory; // GameDirectoryのインスタンス
    public GameObject StageSelectedButton; // Stageを選択して最初に選ばれるボタン
    public GameObject SettingSelectedButton; // Settingを選択して最初に選ばれるボタン
    public GameObject OperatingSelectedButton; // 操作説明を選択して最初に選ばれるボタン
    public GameObject SoundSettingSelectedButton; // サウンド設定を選択して最初に選ばれるボタン
    public GameObject StartSelectButton; // 開始した際初めに選択させるボタン

    private void Start()
    {
        // SoundManagerのインスタンスを取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();
        // GameDirectoryのインスタンスを取得
        gameDirectory = Object.FindFirstObjectByType<GameDirectory>();
        // 初期状態でチェックポイントに戻るボタンを非表示にする
        foreach (var button in returnToCheckpointButtons)
        {
            button.SetActive(false);
        }
    }

    // 設定ボタンがクリックされたときに呼び出される
    public void OnClickSettingBottun()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        Setting.SetActive(true); // 設定画面を表示
        EventSystem.current.SetSelectedGameObject(SettingSelectedButton); // ボタンをセレクトさせる
    }

    // 操作説明ボタンがクリックされたときに呼び出される
    public void OnclickOperatingBottun()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        Operating.SetActive(true); // 操作説明画面を表示
        EventSystem.current.SetSelectedGameObject(OperatingSelectedButton); // ボタンをセレクトさせる
    }

    // BGM設定ボタンがクリックされたときに呼び出される
    public void OnclickBGMSettingBottun()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        SoundSetting.SetActive(true); // サウンド設定画面を表示
        EventSystem.current.SetSelectedGameObject(SoundSettingSelectedButton); // ボタンをセレクトさせる
    }

    // 操作説明画面の戻るボタンがクリックされたときに呼び出される
    public void OnclickBackOperatingBottun()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        Operating.SetActive(false); // 操作説明画面を非表示
        EventSystem.current.SetSelectedGameObject(SettingSelectedButton); // ボタンをセレクトさせる
    }

    // BGM設定画面の戻るボタンがクリックされたときに呼び出される
    public void OnclickBackBGMBottun()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        SoundSetting.SetActive(false); // サウンド設定画面を非表示
        EventSystem.current.SetSelectedGameObject(SettingSelectedButton); // ボタンをセレクトさせる
    }

    // 設定画面の戻るボタンがクリックされたときに呼び出される
    public void OnClickBackButton()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        Setting.SetActive(false); // 設定画面を非表示
        EventSystem.current.SetSelectedGameObject(StartSelectButton); // ボタンをセレクトさせる
    }

    // スタートボタンがクリックされたときに呼び出される
    public void OnClickStartButton()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        StageSelect.SetActive(true); // ステージ選択画面を表示
        EventSystem.current.SetSelectedGameObject(StageSelectedButton); // ボタンをセレクトさせる
    }

    // ステージ選択画面の戻るボタンがクリックされたときに呼び出される
    public void OnClickStageBackButton()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        StageSelect.SetActive(false); // ステージ選択画面を非表示
        EventSystem.current.SetSelectedGameObject(StartSelectButton); // ボタンをセレクトさせる
    }

    // チュートリアルボタンがクリックされたときに呼び出される
    public void OnClickTutorialButton()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        Cursor.visible = false; // カーソルを非表示にする
        SceneManager.LoadScene("Tutorial(Stage1)"); // チュートリアルシーンに遷移
        deadArea.Rerieve(); // DeadAreaのリセット処理を呼び出す
    }


    // ステージ1ボタンがクリックされたときに呼び出される
    public void OnClickStage2Button()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        Cursor.visible = false; // カーソルを非表示にする
        SceneManager.LoadScene("Stage2"); // ステージ1シーンに遷移
        deadArea.Rerieve(); // DeadAreaのリセット処理を呼び出す
    }

    // ステージ3ボタンがクリックされたときに呼び出される
    public void OnClickStage3Button()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        Cursor.visible = false; // カーソルを非表示にする
        SceneManager.LoadScene("Stage3"); // ステージ3シーンに遷移
        deadArea.Rerieve(); // DeadAreaのリセット処理を呼び出す
    }


    // タイトルボタンがクリックされたときに呼び出される
    public void OnClickTitleButton()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        SceneManager.LoadScene("TitleScene"); // タイトルシーンに遷移
        EventSystem.current.SetSelectedGameObject(StartSelectButton); // ボタンをセレクトさせる
    }

    // 終了ボタンがクリックされたときに呼び出される
    public void OnClickExitButton()
    {
        soundManager.PlayClickAudio(); // クリック音を再生
        Application.Quit(); // アプリケーションを終了
    }

    // チェックポイントに戻るボタンがクリックされたときに呼び出される
    public void OnClickReturnToCheckpointButton()
    {
        if (soundManager != null)
        {
            soundManager.PlayClickAudio(); // クリック音を再生
        }

        if (uI_Anim != null)
        {
            uI_Anim.CloseGameoverUI(); // ゲームオーバーUIを閉じる
        }

        Cursor.visible = false; // カーソルを非表示にする

        if (PauseUI != null)
        {
            PauseUI.SetActive(false); // ポーズUIを非表示にする
        }

        Time.timeScale = 1f; // ゲームの時間を通常に戻す

        if (deadArea != null)
        {
            deadArea.Rerieve(); // DeadAreaのリセット処理を呼び出す
        }

        if (gameDirectory != null)
        {
            gameDirectory.ReturnToCheckpoint(); // チェックポイントに戻る処理を呼び出す
        }

        PauseManager pauseManager = Object.FindFirstObjectByType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.ResetPause(); // ポーズ状態をリセットする
        }
    }

    // チェックポイントに戻るボタンを表示するメソッド
    public void ShowReturnToCheckpointButton()
    {
        foreach (var button in returnToCheckpointButtons)
        {
            button.SetActive(true); // チェックポイントに戻るボタンを表示
        }
    }
}
