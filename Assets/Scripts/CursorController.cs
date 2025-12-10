using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("Cursor Settings")]
    public Texture2D customCursor;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;
    
    [Header("Cursor Visibility")]
    public bool hideCursorOnStart = false;
    public bool lockCursor = false;
    
    private void Start()
    {
        // カーソルの初期設定
        if (customCursor != null)
        {
            Cursor.SetCursor(customCursor, hotSpot, cursorMode);
        }
        
        if (hideCursorOnStart)
        {
            Cursor.visible = false;
        }
        
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    private void Update()
    {
        // ESCキーでカーソルロックを解除
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                UnlockCursor();
            }
        }
    }
    
    public void SetCustomCursor(Texture2D cursor, Vector2 hotspot)
    {
        Cursor.SetCursor(cursor, hotspot, cursorMode);
    }
    
    public void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    
    public void ShowCursor()
    {
        Cursor.visible = true;
    }
    
    public void HideCursor()
    {
        Cursor.visible = false;
    }
    
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void ConfineCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
}