using UnityEngine;

namespace TPSBR
{
    /// <summary>
    /// モバイルシミュレーション時にカーソルを強制表示させるためのヘルパークラス
    /// UI ボタンがクリックできない問題を解決します
    /// </summary>
    public class MobileSimulationHelper : MonoBehaviour
    {
        [Header("Mobile Simulation Settings")]
        [Tooltip("モバイルシミュレーション中にカーソルを強制表示するか")]
        public bool forceShowCursorInMobileSimulation = true;
        
        [Tooltip("カーソル表示を切り替えるキー")]
        public KeyCode toggleCursorKey = KeyCode.F1;
        
        [Header("Debug Info")]
        [SerializeField] private bool currentCursorVisible;
        [SerializeField] private CursorLockMode currentLockMode;
        
        private SceneInput sceneInput;
        private bool originalCursorState;
        
        void Start()
        {
            // SceneInput コンポーネントを取得
            sceneInput = FindObjectOfType<SceneInput>();
            
            if (sceneInput == null)
            {
                Debug.LogWarning("[MobileSimulationHelper] SceneInput が見つかりませんでした");
            }
            
            // モバイルシミュレーション中かチェック
            if (Application.isMobilePlatform || IsSimulateMobileInput())
            {
                if (forceShowCursorInMobileSimulation)
                {
                    Debug.Log("[MobileSimulationHelper] モバイルシミュレーション中です。カーソルを表示します。");
                    ShowCursor();
                }
            }
        }
        
        void Update()
        {
            // デバッグ情報更新
            currentCursorVisible = Cursor.visible;
            currentLockMode = Cursor.lockState;
            
            // カーソル表示切り替えキー
            if (Input.GetKeyDown(toggleCursorKey))
            {
                ToggleCursor();
            }
            
            // モバイルシミュレーション中の自動制御
            if (forceShowCursorInMobileSimulation && (Application.isMobilePlatform || IsSimulateMobileInput()))
            {
                // カーソルがロックされていたら強制表示
                if (Cursor.lockState == CursorLockMode.Locked || !Cursor.visible)
                {
                    ShowCursor();
                }
            }
        }
        
        /// <summary>
        /// カーソルを表示する
        /// </summary>
        public void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // SceneInput に UI からカーソル表示を要求
            if (sceneInput != null)
            {
                sceneInput.RequestCursorVisibility(true, ECursorStateSource.UI, true);
            }
            
            Debug.Log("[MobileSimulationHelper] カーソルを表示しました");
        }
        
        /// <summary>
        /// カーソルを非表示にする
        /// </summary>
        public void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // SceneInput から UI のカーソル表示要求を削除
            if (sceneInput != null)
            {
                sceneInput.RequestCursorVisibility(false, ECursorStateSource.UI, true);
            }
            
            Debug.Log("[MobileSimulationHelper] カーソーを非表示にしました");
        }
        
        /// <summary>
        /// カーソー表示を切り替える
        /// </summary>
        public void ToggleCursor()
        {
            if (Cursor.visible)
            {
                HideCursor();
            }
            else
            {
                ShowCursor();
            }
        }
        
        /// <summary>
        /// モバイル入力がシミュレートされているかチェック
        /// </summary>
        private bool IsSimulateMobileInput()
        {
            // Unity の Device Simulator が使用されているかチェック
            #if UNITY_EDITOR
            return UnityEngine.Device.SystemInfo.deviceType == DeviceType.Handheld ||
                   SystemInfo.deviceModel.Contains("Simulator");
            #else
            return false;
            #endif
        }
        
        void OnGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                // デバッグ情報表示
                GUI.color = Color.white;
                string info = $"[Mobile Sim Helper]\n" +
                             $"Cursor Visible: {currentCursorVisible}\n" +
                             $"Lock Mode: {currentLockMode}\n" +
                             $"Mobile Platform: {Application.isMobilePlatform}\n" +
                             $"Toggle Key: {toggleCursorKey}";
                             
                GUI.Label(new Rect(10, 10, 300, 100), info);
            }
        }
    }
}