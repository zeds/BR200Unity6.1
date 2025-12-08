using UnityEngine;
using System.Collections;

namespace TPSBR
{
    /// <summary>
    /// ネームプレートシステムの問題診断用スクリプト
    /// GameシーンのGameObjectにアタッチして使用してください
    /// </summary>
    public class NameplateDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool _enableDebugMode = true;
        [SerializeField] private float _checkInterval = 2.0f;
        
        private float _nextCheckTime = 0f;
        private int _agentCount = 0;
        private int _nameplateCount = 0;
        
        private void Start()
        {
            Debug.Log("[NameplateDebugger] Starting nameplate system diagnostics...");
            
            // NameplateManagerの確認
            CheckNameplateManager();
            
            // NetworkGameの確認
            CheckNetworkGame();
            
            // 定期的なチェックを開始
            StartCoroutine(PeriodicCheck());
        }
        
        private void CheckNameplateManager()
        {
            var manager = NameplateManager.Instance;
            if (manager != null)
            {
                Debug.Log("[NameplateDebugger] NameplateManager found!");
            }
            else
            {
                Debug.LogError("[NameplateDebugger] NameplateManager NOT found! Creating one...");
                GameObject managerObject = new GameObject("NameplateManager");
                managerObject.AddComponent<NameplateManager>();
                Debug.Log("[NameplateDebugger] NameplateManager created manually.");
            }
        }
        
        private void CheckNetworkGame()
        {
            var networkGame = FindObjectOfType<NetworkGame>();
            if (networkGame != null)
            {
                Debug.Log($"[NameplateDebugger] NetworkGame found: {networkGame.name}");
            }
            else
            {
                Debug.LogWarning("[NameplateDebugger] NetworkGame NOT found. This might be normal if not in game yet.");
            }
        }
        
        private IEnumerator PeriodicCheck()
        {
            while (_enableDebugMode)
            {
                yield return new WaitForSeconds(_checkInterval);
                
                // すべてのAgentを検索
                Agent[] agents = FindObjectsOfType<Agent>();
                _agentCount = agents.Length;
                
                Debug.Log($"[NameplateDebugger] === PERIODIC CHECK ===");
                Debug.Log($"[NameplateDebugger] Found {_agentCount} Agent(s) in scene");
                
                foreach (Agent agent in agents)
                {
                    CheckAgent(agent);
                }
                
                // すべてのAgentNameplateを検索
                AgentNameplate[] nameplates = FindObjectsOfType<AgentNameplate>();
                _nameplateCount = nameplates.Length;
                
                Debug.Log($"[NameplateDebugger] Found {_nameplateCount} AgentNameplate(s) in scene");
                
                foreach (AgentNameplate nameplate in nameplates)
                {
                    CheckNameplate(nameplate);
                }
                
                // Canvasの確認
                Canvas[] canvases = FindObjectsOfType<Canvas>();
                int worldCanvasCount = 0;
                foreach (Canvas canvas in canvases)
                {
                    if (canvas.renderMode == RenderMode.WorldSpace)
                    {
                        worldCanvasCount++;
                        if (canvas.name.StartsWith("Nameplate"))
                        {
                            Debug.Log($"[NameplateDebugger] Found nameplate canvas: {canvas.name}, Active: {canvas.gameObject.activeSelf}, Position: {canvas.transform.position}");
                        }
                    }
                }
                Debug.Log($"[NameplateDebugger] Total WorldSpace canvases: {worldCanvasCount}");
                
                // カメラの確認
                if (Camera.main == null)
                {
                    Debug.LogWarning("[NameplateDebugger] Camera.main is null!");
                }
                else
                {
                    Debug.Log($"[NameplateDebugger] Main camera: {Camera.main.name} at {Camera.main.transform.position}");
                }
                
                Debug.Log($"[NameplateDebugger] === END CHECK ===");
            }
        }
        
        private void CheckAgent(Agent agent)
        {
            if (agent == null) return;
            
            string agentInfo = $"Agent: {agent.name}";
            
            // AgentNameplateコンポーネントの確認
            AgentNameplate nameplate = agent.GetComponent<AgentNameplate>();
            if (nameplate != null)
            {
                agentInfo += " [Has Nameplate]";
            }
            else
            {
                agentInfo += " [NO NAMEPLATE!]";
            }
            
            // NetworkObjectの確認
            var networkObject = agent.GetComponent<Fusion.NetworkObject>();
            if (networkObject != null)
            {
                agentInfo += $" InputAuth: {networkObject.InputAuthority}";
                agentInfo += $" HasInput: {networkObject.HasInputAuthority}";
            }
            
            Debug.Log($"[NameplateDebugger] {agentInfo}");
        }
        
        private void CheckNameplate(AgentNameplate nameplate)
        {
            if (nameplate == null) return;
            
            string info = $"Nameplate on: {nameplate.gameObject.name}";
            
            // Canvasの子要素を確認
            Canvas childCanvas = nameplate.GetComponentInChildren<Canvas>();
            if (childCanvas != null)
            {
                info += $" Canvas found: {childCanvas.name}, Active: {childCanvas.gameObject.activeSelf}";
            }
            else
            {
                info += " [NO CANVAS!]";
            }
            
            // Textコンポーネントの確認
            var textComponent = nameplate.GetComponentInChildren<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                info += $" Text: '{textComponent.text}'";
            }
            else
            {
                info += " [NO TEXT!]";
            }
            
            Debug.Log($"[NameplateDebugger] {info}");
        }
        
        private void OnGUI()
        {
            if (!_enableDebugMode) return;
            
            // デバッグ情報をGUIに表示
            GUI.Box(new Rect(10, 10, 300, 100), "Nameplate Debug Info");
            GUI.Label(new Rect(20, 30, 280, 20), $"Agents in scene: {_agentCount}");
            GUI.Label(new Rect(20, 50, 280, 20), $"Nameplates in scene: {_nameplateCount}");
            GUI.Label(new Rect(20, 70, 280, 20), $"Camera.main: {(Camera.main != null ? "OK" : "NULL")}");
        }
    }
}