using UnityEngine;
using UnityEngine.UI;
using System;

public class SimpleButtonTest : MonoBehaviour
{
    private System.Action _jetpackActivationHandler;
    private Button _button;
    private Image _buttonImage;
    
    [SerializeField]
    private Color _activeColor = Color.green;
    [SerializeField]
    private Color _inactiveColor = Color.white;

    void Start()
    {
        Debug.Log("MyButton スクリプトが開始されました！");
        // ボタンコンポーネントを取得
        _button = GetComponent<Button>();
        _buttonImage = GetComponent<Image>();

        // クリックイベントを登録
        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        Debug.Log("ボタンがクリックされました！");
        
        // Call the jetpack activation handler if it's set
        _jetpackActivationHandler?.Invoke();
    }

    public void SetJetpackActivationHandler(System.Action handler)
    {
        _jetpackActivationHandler = handler;
    }

    public void UpdateJetpackStatus(bool isActive)
    {
        if (_buttonImage != null)
        {
            _buttonImage.color = isActive ? _activeColor : _inactiveColor;
        }
    }
}