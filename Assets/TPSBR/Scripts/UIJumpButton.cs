using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPSBR
{
    /// <summary>
    /// UIジャンプボタン - Event Triggerを使用してボタンの押下状態を管理
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(EventTrigger))]
    public class UIJumpButton : MonoBehaviour
    {
        // 静的変数でボタンの押下状態を保持
        private static bool _isPressed = false;
        public static bool IsPressed => _isPressed;

        // コンポーネント参照
        private Button _button;
        private Image _buttonImage;
        private EventTrigger _eventTrigger;

        [Header("Visual Settings")]
        [SerializeField] private Color _normalColor = new Color(1f, 1f, 1f, 0.8f);
        [SerializeField] private Color _pressedColor = new Color(1.0f, 0.0f, 0.0f, 1f);

        [Header("Debug")]
        [SerializeField] private bool _debugMode = true;

        private void Awake()
        {
            // コンポーネントの取得
            _button = GetComponent<Button>();
            _buttonImage = GetComponent<Image>();
            _eventTrigger = GetComponent<EventTrigger>();

            // EventTriggerがない場合は追加
            if (_eventTrigger == null)
            {
                _eventTrigger = gameObject.AddComponent<EventTrigger>();
            }

            // イベントの設定
            SetupEventTriggers();

            // 初期状態
            _isPressed = false;
            UpdateVisual(false);
        }

        private void SetupEventTriggers()
        {
            // 既存のトリガーをクリア
            _eventTrigger.triggers.Clear();

            // PointerDownイベント（ボタンが押された時）
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { OnButtonDown(); });
            _eventTrigger.triggers.Add(pointerDownEntry);

            // PointerUpイベント（ボタンが離された時）
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { OnButtonUp(); });
            _eventTrigger.triggers.Add(pointerUpEntry);

            // PointerExitイベント（ポインタがボタンから出た時）
            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((data) => { OnButtonExit(); });
            _eventTrigger.triggers.Add(pointerExitEntry);

            if (_debugMode)
            {
                Debug.Log("UIJumpButton: Event triggers setup complete");
            }
        }

        // ボタンが押された時
        public void OnButtonDown()
        {
            if (_button != null && _button.interactable)
            {
                _isPressed = true;
                UpdateVisual(true);

                if (_debugMode)
                {
                    Debug.Log("UIJumpButton: Button DOWN - IsPressed = " + _isPressed.ToString());
                }
            }
        }

        // ボタンが離された時
        public void OnButtonUp()
        {
            _isPressed = false;
            UpdateVisual(false);

            if (_debugMode)
            {
                Debug.Log("UIJumpButton: Button UP - IsPressed = " + _isPressed.ToString());
            }
        }

        // ポインタがボタンから出た時
        public void OnButtonExit()
        {
            if (_isPressed)
            {
                _isPressed = false;
                UpdateVisual(false);

                if (_debugMode)
                {
                    Debug.Log("UIJumpButton: Pointer EXIT while pressed - IsPressed = " + _isPressed.ToString());
                }
            }
        }

        private void OnEnable()
        {
            // 有効化時に状態をリセット
            _isPressed = false;
            UpdateVisual(false);

            if (_debugMode)
            {
                Debug.Log("UIJumpButton: Enabled - State reset");
            }
        }

        private void OnDisable()
        {
            // 無効化時に状態をリセット
            _isPressed = false;

            if (_debugMode)
            {
                Debug.Log("UIJumpButton: Disabled - State reset");
            }
        }

        private void OnDestroy()
        {
            // 破棄時に状態をリセット
            _isPressed = false;
        }

        // ビジュアルの更新
        private void UpdateVisual(bool pressed)
        {
            if (_buttonImage != null)
            {
                _buttonImage.color = pressed ? _pressedColor : _normalColor;
            }
        }

        // デバッグ用：Update内で状態を確認
        private void Update()
        {
            if (_debugMode && Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("UIJumpButton: Current state check - IsPressed = " + _isPressed.ToString());
            }
        }

        // テスト用のパブリックメソッド
        public void TestButtonPress()
        {
            Debug.Log("UIJumpButton Test: Button clicked! Current IsPressed = " + _isPressed.ToString());
        }
    }
}