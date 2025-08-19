using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using System;

namespace TPSBR.UI
{
	[System.Serializable]
	public class ResponseData
	{
		public bool success;  // トップレベルに配置
		public Data data;
	}

	[System.Serializable]
	public class Data
	{
		public string message;
		public string timestamp;
	}
	public class UIMainMenuView : UIView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private UIButton _playButton;
		[SerializeField]
		private UIButton _settingsButton;
		[SerializeField]
		private UIButton _creditsButton;
		[SerializeField]
		private UIButton _changeNicknameButton;
		[SerializeField]
		private UIButton _quitButton;
		[SerializeField]
		private UIButton _playerButton;
		[SerializeField]
		private UIPlayer _player;
		[SerializeField]
		private TextMeshProUGUI _agentName;
		[SerializeField]
		private TextMeshProUGUI _applicationVersion;

		// PUBLIC METHODS

		public void OnPlayerButtonPointerEnter()
		{
			Context.PlayerPreview.ShowOutline(true);
		}

		public void OnPlayerButtonPointerExit()
		{
			Context.PlayerPreview.ShowOutline(false);
		}

		// UIView INTEFACE

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_settingsButton.onClick.AddListener(OnSettingsButton);
			_playButton.onClick.AddListener(OnPlayButton);
			_creditsButton.onClick.AddListener(OnCreditsButton);
			_changeNicknameButton.onClick.AddListener(OnChangeNicknameButton);
			_quitButton.onClick.AddListener(OnQuitButton);
			_playerButton.onClick.AddListener(OnPlayerButton);

			_applicationVersion.text = $"Version {Application.version}";
		}

		protected override void OnDeinitialize()
		{
			_settingsButton.onClick.RemoveListener(OnSettingsButton);
			_playButton.onClick.RemoveListener(OnPlayButton);
			_creditsButton.onClick.RemoveListener(OnCreditsButton);
			_changeNicknameButton.onClick.RemoveListener(OnChangeNicknameButton);
			_quitButton.onClick.RemoveListener(OnQuitButton);
			_playerButton.onClick.RemoveListener(OnPlayerButton);

			base.OnDeinitialize();
		}

		protected override void OnOpen()
		{
			base.OnOpen();

			UpdatePlayer();

			Global.PlayerService.PlayerDataChanged += OnPlayerDataChanged;
			Context.PlayerPreview.ShowAgent(Context.PlayerData.AgentID);

			Context.PlayerPreview.ShowOutline(false);
		}

		protected override void OnClose()
		{
			Global.PlayerService.PlayerDataChanged -= OnPlayerDataChanged;

			Context.PlayerPreview.ShowOutline(false);

			base.OnClose();
		}

		protected override bool OnBackAction()
		{
			if (IsInteractable == false)
				return false;

			OnQuitButton();
			return true;
		}

		// PRIVATE METHODS

		private void OnSettingsButton()
		{
			Open<UISettingsView>();
		}

		private void OnPlayButton()
		{

			StartCoroutine(PostJson());

			// // エラーダイアログを表示
			// var errorDialog = Open<UIErrorDialogView>();

			// // エラーメッセージを設定（推測される設定方法）
			// errorDialog.Title.text = "Error";
			// errorDialog.Description.text = "Pls try to update your APP";

			// // ダイアログが閉じられた時の処理
			// errorDialog.HasClosed += () =>
			// {
			// 	// ダイアログが閉じられた後の処理
			// 	Debug.Log("エラーダイアログが閉じられました");
			// };

			// Open<UIMultiplayerView>();
		}

		IEnumerator PostJson()
		{
			Debug.Log("PostJson called");

			// JSON文字列を作成
			string version = Application.version.ToString();
			string response_json = "{\"name\":\"Player888\",\"version\":\"0.0.1\"}";
			byte[] bodyRaw = Encoding.UTF8.GetBytes(response_json);

			// POSTリクエスト作成
			UnityWebRequest request = new UnityWebRequest("https://find-vietnam.com/api/v1/playbutton", "POST");
			// UnityWebRequest request = new UnityWebRequest("http://localhost:3000/api/v1/playbutton", "POST");
			request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = new DownloadHandlerBuffer();

			// Content-Typeを application/json に設定
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			if (request.result != UnityWebRequest.Result.Success)
				Debug.LogError(request.error);
			else
			{
				string json = request.downloadHandler.text;
				ResponseData response = JsonUtility.FromJson<ResponseData>(json);
				bool isSuccess = response.success;
				string message = response.data.message;

				// エラーダイアログを表示
				var errorDialog = Open<UIInfoDialogView>();

				// エラーメッセージを設定（推測される設定方法）
				errorDialog.Title.text = isSuccess ? "Success" : "Error";
				errorDialog.Description.text = message;

				// ダイアログが閉じられた時の処理
				errorDialog.HasClosed += () =>
				{
					// ダイアログが閉じられた後の処理
					if (isSuccess)
					{
						Open<UIMultiplayerView>();
					}
					else
					{
						Debug.LogError("Play button action failed: " + message);
					}
				};

			}
			// Debug.Log(request.downloadHandler.text);
		}

		private void OnCreditsButton()
		{
			Open<UICreditsView>();
		}

		private void OnChangeNicknameButton()
		{
			var changeNicknameView = Open<UIChangeNicknameView>();
			changeNicknameView.SetData("CHANGE NICKNAME", false);
		}

		private void OnQuitButton()
		{
			var dialog = Open<UIYesNoDialogView>();

			dialog.Title.text = "EXIT GAME";
			dialog.Description.text = "Are you sure you want to exit the game?";

			dialog.YesButtonText.text = "EXIT";
			dialog.NoButtonText.text = "CANCEL";

			dialog.HasClosed += (result) =>
			{
				if (result == true)
				{
					SceneUI.Scene.Quit();
				}
			};
		}

		private void OnPlayerButton()
		{
			var agentSelection = Open<UIAgentSelectionView>();
			agentSelection.BackView = this;

			Close();
		}

		private void OnPlayerDataChanged(PlayerData playerData)
		{
			UpdatePlayer();
		}

		private void UpdatePlayer()
		{
			_player.SetData(Context, Context.PlayerData);
			Context.PlayerPreview.ShowAgent(Context.PlayerData.AgentID);

			var setup = Context.Settings.Agent.GetAgentSetup(Context.PlayerData.AgentID);
			_agentName.text = setup != null ? $"Playing as {setup.DisplayName}" : string.Empty;
		}
	}
}