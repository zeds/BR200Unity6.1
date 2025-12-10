using UnityEngine;
using TMPro;

namespace TPSBR
{
	/// <summary>
	/// Player name plate that always faces the camera
	/// </summary>
	public class NamePlate : MonoBehaviour
	{
		[Header("References")]
		[SerializeField]
		private TextMeshProUGUI _nameText;

		[SerializeField]
		private CanvasGroup _canvasGroup;

		[Header("Settings")]

		[SerializeField]
		private float _fadeStartDistance = 50f;

		[SerializeField]
		private float _maxDistance = 100f;

		private Transform _target;
		private Camera _camera;

		private bool _loggedOnce = false;
		private Agent _agent;

		// Cached components
		private Canvas _canvas;
		private RectTransform _canvasRect;
		private RectTransform _textRect;


		// PUBLIC METHODS

		public void Initialize(Agent agent, string playerName, Camera camera)
		{
			Debug.Log("[NamePlate] Initialize called - Player: " + playerName);

			_agent = agent;
			_target = agent.Character.ThirdPersonView.HeadTransform;
			_camera = camera;

			// Cache Canvas component
			_canvas = GetComponentInChildren<Canvas>();
			if (_canvas != null)
			{
				_canvas.renderMode = RenderMode.WorldSpace;
				_canvas.worldCamera = camera;  // CRITICAL: Set camera for rendering!
				_canvas.sortingOrder = 1000;

				// Set proper scale for World Space Canvas
				_canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

				Debug.Log($"[NamePlate] Canvas configured - Camera: {camera.name}, Scale: {_canvas.transform.localScale}");
			}

			if (_nameText != null)
			{
				_nameText.text = playerName;
				Debug.Log("[NamePlate] Font size from Prefab: " + _nameText.fontSize);
				_nameText.alignment = TMPro.TextAlignmentOptions.Center;
				Debug.Log("[NamePlate] Text configured");
			}
			else
			{
				Debug.LogError("[NamePlate] _nameText is NULL!");
			}

			if (_canvasGroup != null)
			{
				_canvasGroup.alpha = 1f;
			}

			Debug.Log("[NamePlate] Initialization complete");
		}

		public void SetColor(Color color)
		{
			if (_nameText != null)
			{
				_nameText.color = color;
			}
		}

		public void SetNameText(string playerName)
		{
			if (_nameText != null)
			{
				_nameText.text = playerName;
			}
		}

		private void LateUpdate()
		{
			// Log only first few frames for debugging
			if (Time.frameCount % 100 == 0)
			{
				// Debug.Log($"[NamePlate] LateUpdate - Frame: {Time.frameCount}, Target: {(_target != null ? "OK" : "NULL")}, Camera: {(_camera != null ? "OK" : "NULL")}");
			}

			if (_target == null)
			{
				Debug.LogWarning("[NamePlate] LateUpdate - _target is NULL!");
				return;
			}

			if (_camera == null)
			{
				Debug.LogWarning("[NamePlate] LateUpdate - _camera is NULL!");
				return;
			}

			// X, Z座標はプレイヤーの頭を追従、Y座標は頭から0.5m上にオフセット
			Vector3 targetPosition = _target.position;
			targetPosition.y = _target.position.y + 0.35f;

			transform.position = targetPosition;

			// Billboard effect - カメラと完全に同じ向きにする
			transform.rotation = _camera.transform.rotation;

			// Distance-based fading
			float distance = Vector3.Distance(_camera.transform.position, transform.position);

			// Log distance occasionally
			if (Time.frameCount % 100 == 0)
			{
				Debug.Log($"[NamePlate] Distance to camera: {distance:F1}m, Position: {transform.position}, Alpha will be: {(distance > _maxDistance ? 0f : (distance > _fadeStartDistance ? 1f - ((distance - _fadeStartDistance) / (_maxDistance - _fadeStartDistance)) : 1f)):F2}");
			}

			float alpha = 1f;

			if (distance > _maxDistance)
			{
				alpha = 0f;
			}
			else if (distance > _fadeStartDistance)
			{
				alpha = 1f - ((distance - _fadeStartDistance) / (_maxDistance - _fadeStartDistance));
			}

			if (_canvasGroup != null)
			{
				_canvasGroup.alpha = alpha;
			}

			// Apply alpha to text color while preserving the original color
			if (_nameText != null)
			{
				Color currentColor = _nameText.color;
				currentColor.a = alpha;
				_nameText.color = currentColor;
			}
		}

		private void OnDestroy()
		{
			_target = null;
			_camera = null;
			_agent = null;
		}
	}
}
