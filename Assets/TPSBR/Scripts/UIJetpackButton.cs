using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TPSBR
{
    public class UIJetpackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Color _normalColor = Color.white;
        [SerializeField]
        private Color _pressedColor = Color.gray;
        [SerializeField]
        private Color _activeColor = Color.green;

        private Image _buttonImage;
        private bool _isPressed = false;
        private bool _wasTriggered = false;
        private Agent _connectedAgent;

        public bool IsPressed => _isPressed;
        public bool WasTriggered
        {
            get
            {
                bool result = _wasTriggered;
                _wasTriggered = false;
                return result;
            }
        }

        void Start()
        {
            _buttonImage = GetComponent<Image>();
            if (_buttonImage != null)
            {
                _buttonImage.color = _normalColor;
            }

            Debug.Log("UIJetpackButton: 初期化完了");
        }

        void Update()
        {
            // Agentとの接続を維持
            if (_connectedAgent == null)
            {
                FindAgent();
            }

            // Jetpackの状態に応じて色を更新
            UpdateVisualState();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            _wasTriggered = true;
            
            if (_buttonImage != null)
            {
                _buttonImage.color = _pressedColor;
            }

            Debug.Log("UIJetpackButton: ボタンが押されました");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
            
            UpdateVisualState();
            
            Debug.Log("UIJetpackButton: ボタンが離されました");
        }

        private void FindAgent()
        {
            // 入力権限を持つAgentを探す
            Agent[] agents = FindObjectsByType<Agent>(FindObjectsSortMode.None);
            foreach (Agent agent in agents)
            {
                if (agent.HasInputAuthority)
                {
                    _connectedAgent = agent;
                    Debug.Log("UIJetpackButton: Agentに接続しました");
                    break;
                }
            }
        }

        private void UpdateVisualState()
        {
            if (_buttonImage == null) return;

            if (_isPressed)
            {
                _buttonImage.color = _pressedColor;
            }
            else if (_connectedAgent != null && _connectedAgent.Jetpack != null && _connectedAgent.Jetpack.IsActive)
            {
                _buttonImage.color = _activeColor;
            }
            else
            {
                _buttonImage.color = _normalColor;
            }
        }

        // モバイル入力システムから呼び出される
        public bool GetJetpackToggleInput()
        {
            return WasTriggered;
        }

        // デバッグ用
        public void TriggerJetpack()
        {
            _wasTriggered = true;
            Debug.Log("UIJetpackButton: 手動でトリガーされました");
        }
    }
}