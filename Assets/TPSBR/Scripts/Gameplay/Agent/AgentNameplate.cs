using UnityEngine;
using UnityEngine.UI;
using Fusion;

namespace TPSBR
{
	/// <summary>
	/// Agent（プレイヤーキャラクター）の頭上に名前を表示するコンポーネント
	/// </summary>
	public class AgentNameplate : MonoBehaviour
	{
		[Header("Nameplate Settings")]
		[SerializeField] private float _heightOffset = 2.0f;
		[SerializeField] private float _canvasScale = 0.01f;
		[SerializeField] private Color _localPlayerColor = Color.green;
		[SerializeField] private Color _otherPlayerColor = Color.white;

		[Header("Visibility")]
		[SerializeField] private float _maxVisibleDistance = 50f;
		[SerializeField] private bool _hideForLocalPlayer = false;

		private GameObject _nameplateCanvas;
		private Text _nameText;
		private Transform _cameraTransform;
		private bool _isInitialized = false;

		// Agentから渡される情報
		private PlayerRef _playerRef;
		private Player _player;
		private bool _isLocalPlayer;
		private NetworkRunner _runner;

		/// <summary>
		/// Agentから呼ばれる初期化メソッド
		/// </summary>
		public void Initialize(NetworkRunner runner, PlayerRef playerRef, Player player, bool isLocalPlayer)
		{
			Debug.Log($"[AgentNameplate] Initialize called - PlayerRef: {playerRef}, IsLocal: {isLocalPlayer}");

			_runner = runner;
			_playerRef = playerRef;
			_player = player;
			_isLocalPlayer = isLocalPlayer;

			// カメラの参照を取得
			if (Camera.main != null)
			{
				_cameraTransform = Camera.main.transform;
				Debug.Log($"[AgentNameplate] Main camera found: {_cameraTransform.name}");
			}
			else
			{
				Debug.LogWarning("[AgentNameplate] Camera.main is null!");
			}

			// ネームプレートUIを作成
			Debug.Log("[AgentNameplate] Creating nameplate UI...");
			CreateNameplateUI();

			// 初期化完了
			_isInitialized = true;
			Debug.Log($"[AgentNameplate] Initialization complete. Canvas active: {_nameplateCanvas?.activeSelf}");

			// 名前を設定
			UpdateDisplayName();
		}

		private void CreateNameplateUI()
		{
			try
			{
				// Canvas GameObject を作成
				_nameplateCanvas = new GameObject($"Nameplate_{_playerRef}");
				_nameplateCanvas.transform.SetParent(transform);
				_nameplateCanvas.transform.localPosition = new Vector3(0, _heightOffset, 0);
				_nameplateCanvas.transform.localScale = Vector3.one * _canvasScale;
				Debug.Log($"[AgentNameplate] Canvas created: {_nameplateCanvas.name} at position {_nameplateCanvas.transform.position}");

				// Canvas コンポーネントを追加
				Canvas canvas = _nameplateCanvas.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.WorldSpace;
				Debug.Log($"[AgentNameplate] Canvas render mode set to WorldSpace");

				// CanvasScaler を追加
				CanvasScaler scaler = _nameplateCanvas.AddComponent<CanvasScaler>();
				scaler.dynamicPixelsPerUnit = 10f;

				// Text GameObject を作成
				GameObject textObject = new GameObject("NameText");
				textObject.transform.SetParent(_nameplateCanvas.transform);
				textObject.transform.localPosition = Vector3.zero;
				textObject.transform.localScale = Vector3.one;

				// RectTransform の設定
				RectTransform rectTransform = textObject.AddComponent<RectTransform>();
				rectTransform.sizeDelta = new Vector2(200, 50);
				rectTransform.anchoredPosition = Vector2.zero;

				// Text コンポーネントを追加
				_nameText = textObject.AddComponent<Text>();
				_nameText.text = "Player";
				_nameText.fontSize = 36;
				_nameText.alignment = TextAnchor.MiddleCenter;
				_nameText.color = _otherPlayerColor;
				_nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
				Debug.Log($"[AgentNameplate] Text component created with text: {_nameText.text}");

				// シャドウを追加
				var shadow = textObject.AddComponent<Shadow>();
				shadow.effectColor = Color.black;
				shadow.effectDistance = new Vector2(2, -2);

				Debug.Log($"[AgentNameplate] UI creation complete");
			}
			catch (System.Exception e)
			{
				Debug.LogError($"[AgentNameplate] Error creating UI: {e.Message}\n{e.StackTrace}");
			}
		}

		private void UpdateDisplayName()
		{
			if (_nameText == null)
			{
				Debug.LogWarning("[AgentNameplate] _nameText is null in UpdateDisplayName");
				return;
			}

			string nameToDisplay = "";

			// プレイヤー情報がある場合
			if (_player != null)
			{
				// Nicknameが設定されていればそれを使用
				if (!string.IsNullOrEmpty(_player.Nickname))
				{
					nameToDisplay = _player.Nickname;
					Debug.Log($"[AgentNameplate] Using player nickname: {nameToDisplay}");
				}
			}

			// デフォルトの名前
			if (string.IsNullOrEmpty(nameToDisplay))
			{
				nameToDisplay = $"Player {_playerRef}";
				Debug.Log($"[AgentNameplate] Using default name: {nameToDisplay}");
			}

			_nameText.text = nameToDisplay;

			// 色の設定
			if (_isLocalPlayer)
			{
				_nameText.color = _localPlayerColor;
				Debug.Log($"[AgentNameplate] Set local player color (green)");

				// 自分の名前を隠すオプション
				if (_hideForLocalPlayer && _nameplateCanvas != null)
				{
					_nameplateCanvas.SetActive(false);
					Debug.Log("[AgentNameplate] Hiding nameplate for local player");
				}
			}
			else
			{
				_nameText.color = _otherPlayerColor;
				Debug.Log($"[AgentNameplate] Set other player color (white)");
			}

			Debug.Log($"[AgentNameplate] Name updated to: {_nameText.text}, Color: {_nameText.color}");
		}

		private void LateUpdate()
		{
			if (!_isInitialized || _nameplateCanvas == null)
				return;

			if (_cameraTransform == null)
			{
				// カメラを再取得
				if (Camera.main != null)
				{
					_cameraTransform = Camera.main.transform;
				}
				else
				{
					return;
				}
			}

			// 距離に基づく表示制御
			float distance = Vector3.Distance(transform.position, _cameraTransform.position);
			bool shouldShow = distance <= _maxVisibleDistance;

			// 自分のキャラクターの場合の制御
			if (_isLocalPlayer && _hideForLocalPlayer)
			{
				shouldShow = false;
			}

			if (_nameplateCanvas.activeSelf != shouldShow)
			{
				_nameplateCanvas.SetActive(shouldShow);
			}

			// カメラに向かって回転（ビルボード効果）
			if (shouldShow)
			{
				// カメラの方向を向くように回転（反転させる）
				Vector3 lookDirection = _cameraTransform.position - _nameplateCanvas.transform.position;
				if (lookDirection != Vector3.zero)
				{
					_nameplateCanvas.transform.rotation = Quaternion.LookRotation(-lookDirection);
				}

				// 距離に応じたスケーリング
				float scaleFactor = Mathf.Clamp01(distance / 20f);
				float scale = Mathf.Lerp(_canvasScale * 0.5f, _canvasScale, scaleFactor);
				_nameplateCanvas.transform.localScale = Vector3.one * scale;
			}
		}

		private void OnDestroy()
		{
			if (_nameplateCanvas != null)
			{
				Destroy(_nameplateCanvas);
			}
		}
	}
}