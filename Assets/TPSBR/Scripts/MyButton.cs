using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
// Add the correct namespace for UIMobileInputView
using TPSBR.UI; // Change 'TPSBR.UI' to the actual namespace if different

namespace TPSBR
{
    public class MyButton : MonoBehaviour
    {
        private Button _button;
        private Image _buttonImage;

        [SerializeField]
        private Color _activeColor = Color.green;
        [SerializeField]
        private Color _inactiveColor = Color.white;
        [SerializeField]
        private Color _pressedColor = Color.gray;

        // イベント追加
        [SerializeField]
        private UnityEvent _onButtonDown = new UnityEvent();

        // モバイル入力シミュレーションの設定
        [SerializeField]
        private bool _simulateMobileInput = false;

        public UnityEvent OnButtonDown() => _onButtonDown;

        private bool _isPressed = false;
        private float _pressStartTime = 0f;

        // 静的プロパティ - AgentInputから参照される
        private static bool _staticIsPressed = false;
        public static bool IsPressed => _staticIsPressed;

        void Start()
        {
            Debug.Log("MyButton: 初期化開始 - Xキー入力シミュレーション版");

            // ボタンコンポーネントを取得
            _button = GetComponent<Button>();
            _buttonImage = GetComponent<Image>();

            // クリックイベントを登録
            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClick);
            }

            if (_buttonImage != null)
            {
                _buttonImage.color = _inactiveColor;
            }
        }

        void Update()
        {
            // ボタンが押されている間、Xキーが押されているのと同じ状態を維持
            if (_isPressed)
            {
                SimulateXKeyPressed();

                // 視覚的フィードバック - 押下状態を維持
                if (_buttonImage != null && Time.time - _pressStartTime < 0.2f)
                {
                    _buttonImage.color = _pressedColor;
                }
            }

            // jetpackの状態に応じてボタンの色を更新
            UpdateButtonVisual();
        }

        void OnButtonClick()
        {
            Debug.Log("MyButton: ボタンがクリックされました - Xキー入力をシミュレート");

            // ボタン押下状態を開始
            _isPressed = true;
            _staticIsPressed = true; // 静的プロパティも更新
            _pressStartTime = Time.time;

            // UnityEventを発火
            _onButtonDown?.Invoke();

            // 短時間後に押下状態を解除
            Invoke(nameof(ReleaseButton), 0.1f);
        }

        private void ReleaseButton()
        {
            _isPressed = false;
            _staticIsPressed = false; // 静的プロパティも更新
        }

        private void SimulateXKeyPressed()
        {
            // UIMobileInputViewと連携 - Context.Settingsの代わりに直接判定
            if (Application.isMobilePlatform || _simulateMobileInput)
            {
                TriggerMobileJetpackInput();
            }

            // AgentInputを直接操作してXキー入力をシミュレート
            Agent agent = FindLocalAgent();
            if (agent != null && agent.AgentInput != null)
            {
                try
                {
                    // 現在のrender inputを取得
                    var renderInput = agent.AgentInput.RenderInput;

                    // ToggleJetpackアクションを有効にする（Xキーと同様）
                    renderInput.ToggleJetpack = true;

                    // 入力を設定
                    agent.AgentInput.SetRenderInput(renderInput, false);

                    Debug.Log("MyButton: Xキー相当の入力を送信しました");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"MyButton: 入力シミュレーションエラー: {e.Message}");

                    // フォールバック: 直接jetpackを操作
                    DirectJetpackControl(agent);
                }
            }
            else
            {
                Debug.LogWarning("MyButton: ローカルAgentが見つかりません");
            }
        }

        private void TriggerMobileJetpackInput()
        {
            // UIMobileInputViewを見つけてjetpack入力をトリガー
            var mobileInputView = FindFirstObjectByType<UIMobileInputView>();
            if (mobileInputView != null)
            {
                // UIMobileInputViewにTriggerJetpackToggleメソッドがある場合
                try
                {
                    // リフレクションを使用してメソッドを呼び出し（メソッドが存在しない場合のフォールバック）
                    var method = mobileInputView.GetType().GetMethod("TriggerJetpackToggle");
                    if (method != null)
                    {
                        method.Invoke(mobileInputView, null);
                        Debug.Log("MyButton: UIMobileInputViewにjetpack入力を送信");
                    }
                    else
                    {
                        Debug.LogWarning("MyButton: UIMobileInputView.TriggerJetpackToggleメソッドが見つかりません");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"MyButton: UIMobileInputView呼び出しエラー: {e.Message}");
                }
            }
        }

        private void DirectJetpackControl(Agent agent)
        {
            if (agent?.Jetpack == null) return;

            Debug.Log("MyButton: 直接jetpack制御を実行");

            if (agent.Jetpack.IsActive)
            {
                agent.Jetpack.Deactivate();
                Debug.Log("MyButton: Jetpack停止");
            }
            else
            {
                // Xキーと同じ条件でjetpackを起動
                if (CanActivateJetpack(agent))
                {
                    bool success = agent.Jetpack.Activate();
                    Debug.Log($"MyButton: Jetpack起動試行 - 結果: {success}");

                    if (!success)
                    {
                        LogJetpackStatus(agent);
                    }
                }
                else
                {
                    Debug.Log("MyButton: Jetpack起動条件が満たされていません");
                    LogJetpackStatus(agent);
                }
            }
        }

        private Agent FindLocalAgent()
        {
            // 入力権限を持つAgentを最優先で探す
            Agent[] agents = FindObjectsByType<Agent>(FindObjectsSortMode.None);

            foreach (Agent agent in agents)
            {
                if (agent.HasInputAuthority)
                {
                    return agent;
                }
            }

            // 入力権限がない場合は最初のAgentを返す
            return agents.Length > 0 ? agents[0] : null;
        }

        private bool CanActivateJetpack(Agent agent)
        {
            if (agent?.Jetpack == null) return false;
            if (agent.Jetpack.Fuel <= 0) return false;
            if (agent.Character?.AnimationController == null) return false;

            return agent.Character.AnimationController.CanSwitchWeapons(true);
        }

        private void LogJetpackStatus(Agent agent)
        {
            if (agent?.Jetpack == null) return;

            Debug.Log($"MyButton Debug Info:");
            Debug.Log($"  Jetpack燃料: {agent.Jetpack.Fuel}/{agent.Jetpack.MaxFuel}");
            Debug.Log($"  Jetpack有効: {agent.Jetpack.IsActive}");
            Debug.Log($"  Jetpack動作中: {agent.Jetpack.IsRunning}");

            if (agent.Character?.AnimationController != null)
            {
                Debug.Log($"  武器切替可能: {agent.Character.AnimationController.CanSwitchWeapons(true)}");
            }
        }

        private void UpdateButtonVisual()
        {
            if (_buttonImage == null) return;

            Agent agent = FindLocalAgent();
            if (agent?.Jetpack != null)
            {
                bool isActive = agent.Jetpack.IsActive;

                // 押下状態でない場合のみ色を更新
                if (!_isPressed)
                {
                    _buttonImage.color = isActive ? _activeColor : _inactiveColor;
                }
            }
        }

        // UIGameplayViewとの互換性のためのメソッド
        public void SetJetpackActivationHandler(System.Action handler)
        {
            Debug.Log("MyButton: SetJetpackActivationHandler呼び出し（Xキーシミュレーション版では無視）");
        }

        public void UpdateJetpackStatus(bool isActive)
        {
            // UIGameplayViewからの状態更新を受け取る
            if (_buttonImage != null && !_isPressed)
            {
                _buttonImage.color = isActive ? _activeColor : _inactiveColor;
            }
        }

        // 公開メソッド
        public void TriggerJetpackToggle()
        {
            OnButtonClick();
        }

        // デバッグ用
        [ContextMenu("Test X Key Simulation")]
        public void TestXKeySimulation()
        {
            OnButtonClick();
        }

        [ContextMenu("Show Jetpack Status")]
        public void ShowJetpackStatus()
        {
            Agent agent = FindLocalAgent();
            if (agent == null)
            {
                Debug.Log("MyButton: Agentが見つかりません");
                return;
            }

            Debug.Log($"MyButton: Agent = {agent.name}");
            Debug.Log($"MyButton: 入力権限 = {agent.HasInputAuthority}");
            LogJetpackStatus(agent);
        }

        // キーボード入力をテストするためのメソッド
        [ContextMenu("Test Keyboard X Key")]
        public void TestKeyboardXKey()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                bool xPressed = keyboard.xKey.isPressed;
                Debug.Log($"MyButton: 実際のXキー状態 = {xPressed}");
            }
        }
    }
}