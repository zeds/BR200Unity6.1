using UnityEngine;
using System.Collections.Generic;
using Fusion;

namespace TPSBR
{
    /// <summary>
    /// ゲーム内のすべてのプレイヤーのネームプレートを管理するマネージャー
    /// </summary>
    public class NameplateManager : MonoBehaviour
    {
        private static NameplateManager _instance;
        public static NameplateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<NameplateManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("NameplateManager");
                        _instance = go.AddComponent<NameplateManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Default Settings")]
        [SerializeField] private bool _showNameplates = true;
        [SerializeField] private bool _showLocalPlayerNameplate = false;
        [SerializeField] private float _defaultMaxDistance = 50f;
        
        private Dictionary<PlayerRef, AgentNameplate> _nameplates = new Dictionary<PlayerRef, AgentNameplate>();
        private NetworkGame _networkGame;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // NetworkGameの参照を取得
            _networkGame = FindObjectOfType<NetworkGame>();
        }

        /// <summary>
        /// Agentがスポーンされた時に呼ばれる
        /// </summary>
public void RegisterAgent(Agent agent)
        {
            if (agent == null)
                return;

            // Agent側でネームプレートの初期化は完了しているので、
            // ここでは単に参照を保持するだけ
            PlayerRef playerRef = agent.Object.InputAuthority;
            
            AgentNameplate nameplate = agent.GetComponent<AgentNameplate>();
            if (nameplate != null)
            {
                if (!_nameplates.ContainsKey(playerRef))
                {
                    _nameplates[playerRef] = nameplate;
                }
                else
                {
                    _nameplates[playerRef] = nameplate;
                }
                
                Debug.Log($"[NameplateManager] Registered nameplate for {playerRef}");
            }
        }

        /// <summary>
        /// Agentがデスポーンされた時に呼ばれる
        /// </summary>
        public void UnregisterAgent(PlayerRef playerRef)
        {
            if (_nameplates.ContainsKey(playerRef))
            {
                _nameplates.Remove(playerRef);
            }
        }

        /// <summary>
        /// プレイヤーの名前を更新
        /// </summary>
public void UpdatePlayerName(PlayerRef playerRef)
        {
            // 新しい設計では、名前の更新はAgentNameplate内で自動的に行われるため、
            // このメソッドは互換性のために残すが、実際には何もしない
            Debug.Log($"[NameplateManager] UpdatePlayerName called for {playerRef} (no action needed)");
        }

        /// <summary>
        /// すべてのネームプレートの表示を切り替え
        /// </summary>
        public void ToggleNameplates()
        {
            _showNameplates = !_showNameplates;
            foreach (var nameplate in _nameplates.Values)
            {
                if (nameplate != null && nameplate.gameObject != null)
                {
                    nameplate.gameObject.SetActive(_showNameplates);
                }
            }
        }

        /// <summary>
        /// 設定を取得
        /// </summary>
        public bool ShowNameplates => _showNameplates;
        public bool ShowLocalPlayerNameplate => _showLocalPlayerNameplate;
        public float DefaultMaxDistance => _defaultMaxDistance;
    }
}